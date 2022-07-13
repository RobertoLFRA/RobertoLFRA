using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.Web.Deployment
{
  internal static class CredentialStoreHelper
  {
    public static void GetCredentialsFromStore(
      string target,
      out string userName,
      out string password)
    {
      IntPtr credential = IntPtr.Zero;
      try
      {
        if (!CredentialStoreHelper.NativeMethods.CredRead(target, CredentialStoreHelper.NativeMethods.CRED_TYPE.GENERIC, 0U, out credential))
          throw new Win32Exception(Marshal.GetLastWin32Error());
        CredentialStoreHelper.NativeMethods.NativeCredential structure = (CredentialStoreHelper.NativeMethods.NativeCredential) Marshal.PtrToStructure(credential, typeof (CredentialStoreHelper.NativeMethods.NativeCredential));
        userName = structure.userName;
        if (structure.credentialBlob != IntPtr.Zero)
          password = Marshal.PtrToStringUni(structure.credentialBlob, (int) ((long) structure.credentialBlobSize / (long) Marshal.SystemDefaultCharSize));
        else
          password = (string) null;
      }
      finally
      {
        if (credential != IntPtr.Zero)
          CredentialStoreHelper.NativeMethods.CredFree(credential);
      }
    }

    public static void WriteCredentialsToStore(string target, string userName, string password)
    {
      CredentialStoreHelper.NativeMethods.NativeCredential Credential = new CredentialStoreHelper.NativeMethods.NativeCredential();
      IntPtr zero = IntPtr.Zero;
      try
      {
        Credential.comment = target;
        Credential.credentialBlobSize = (uint) (Marshal.SystemDefaultCharSize * password.Length);
        Credential.credentialBlob = Marshal.StringToCoTaskMemAuto(password);
        Credential.persist = CredentialStoreHelper.NativeMethods.CRED_PERSIST.LOCAL_MACHINE;
        Credential.targetAlias = (string) null;
        Credential.type = 1U;
        Credential.targetName = target;
        Credential.userName = userName;
        if (!CredentialStoreHelper.NativeMethods.CredWrite(ref Credential, 0U))
          throw new Win32Exception(Marshal.GetLastWin32Error());
      }
      finally
      {
        if (Credential.credentialBlob != IntPtr.Zero)
          Marshal.FreeCoTaskMem(Credential.credentialBlob);
        if (zero != IntPtr.Zero)
          CredentialStoreHelper.NativeMethods.CredFree(zero);
      }
    }

    private static class NativeMethods
    {
      public const int ERROR_NOT_FOUND = 1168;

      [DllImport("advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CredRead(
        [MarshalAs(UnmanagedType.LPWStr)] string targetName,
        [MarshalAs(UnmanagedType.U4)] CredentialStoreHelper.NativeMethods.CRED_TYPE type,
        [MarshalAs(UnmanagedType.U4)] uint flags,
        out IntPtr credential);

      [DllImport("advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CredWrite(
        ref CredentialStoreHelper.NativeMethods.NativeCredential Credential,
        [MarshalAs(UnmanagedType.U4)] uint flags);

      [DllImport("advapi32.dll")]
      public static extern void CredFree(IntPtr buffer);

      public enum CRED_TYPE : uint
      {
        GENERIC = 1,
        DOMAIN_PASSWORD = 2,
        DOMAIN_CERTIFICATE = 3,
        DOMAIN_VISIBLE_PASSWORD = 4,
      }

      public enum CRED_PERSIST : uint
      {
        NONE,
        SESSION,
        LOCAL_MACHINE,
        ENTERPRISE,
      }

      [SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct NativeCredential
      {
        public uint flags;
        public uint type;
        public string targetName;
        public string comment;
        public int lastWritten_lowDateTime;
        public int lastWritten_highDateTime;
        public uint credentialBlobSize;
        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        public IntPtr credentialBlob;
        public CredentialStoreHelper.NativeMethods.CRED_PERSIST persist;
        public uint attributeCount;
        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        public IntPtr attributes;
        public string targetAlias;
        public string userName;
      }
    }
  }
}
