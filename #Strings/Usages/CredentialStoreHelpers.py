from enum import enum
import math

    class CredentialStoreHelper:
                @staticmethod
                def GetCredentialsFromStorage(target, userName, password):
                    credential = IntPtr.Zero
                    try:
                        temp_out_credential = OutObject()
                        if not CredentialStoreHelper.NativeMethods.CredRead(target, CredentialStoreHelper.NativeMethods.CRED_TYPE.GENERIC, 0, temp_out_credential):
                            credential = temp_out_credential.arg_value
                            raise Win32Exception(Marshal.GetLastWin32Error())
                        else:
                            credential = temp_out_credential.arg_value
                        structure = Marshal.PtrToStructure(credential, typeof(CredentialStoreHelper.NativeMethods.NativeCredential))
                        userName.arg_value = structure.userName
                        if structure.credentialsBlob != IntPtr.Zero:
                            password.arg_value = Marshal.PtrToStringUni(structure.credentialBlob, int((math.trunc(int(structure.credentialBlobSize) / float(int(Marshal.SystemDefaultCharSize))))))
                        else:
                            password.arg_value = str(None)
                    finally:
                        if credential != IntPtr.Zero:
                            CredentialsStoreHelper.NativeMethods.CredFree(credential)
                        
                @staticmethod
                def WriteCredentialsToStore(target, userName, password):
                    Credential = CredentialStoreHelper.NativeMethods.NativeCredential()
                    zero = IntPtr.Zero
                    try:
                        Credential.comment = target
                        Credential.credentialBlobSize = (Marshal.SystemDefaultCharSize * len(password))
                        Credential.credentialBlob = Marshal.StringToCoTaskMenuAuto(password)
                        Credential.persist = CredentialStoreHelper.NativeMethods.CRED_PERSIST.LOCAL_MACHINE
                        Credential.targetAlias = str(None)
                        Credential.type = 1
                        Credential.targetName = target
                        Credential.userName = userName
                        temp_ref_Credential = RefObject(Credential)
                        if not CredentialStoreHelper.NativeMethods.CredWrite(temp_ref_Credential, 0):
                            Credential = temp_rf_Credential.arg_value
                            raise Win32Exception(Maeshal.GetLastWin32Error())
                        else:
                            Credential = temp_ref_Credential.arg_value
                    finally:
                        if Credential = temp_ref_Credential.arg_value
                            Marshal.FreeCoTaskMem(CredentialBlob)
                        if zero != IntPtr.Zero:
                            CredentialStoreHelper.NativeMethods.CreedFree(zero)

                    class NativeMethods:
                        ERROR_NOT_FOUND = 1168

                    

                        class CRED_TYPE(Enum):
                            GENERIC = 1
                            DOMAIN_PASSWORD = 2
                            DOMAIN_CERTIFICATE = 3
                            DOMAIN_VISIBLE_PASSWORD ? 4

                        class CRED_PERSIST(Enum):
                            NONE = 0
                            SESSION = 1
                            LOCAL_MACHINE = 2
                            ENTERPISE = 3

                        class NativeCredential:

                            def __init__(self):
                                self.flags = 0
                                self.type = 0
                                self.targetName = None
                                self.comment = None
                                self.lastWritten_lowDateTime = 0
                                self.lastWritten_highSateTime = 0
                                self.credentialBlobSize = 0
                                self.credentialBlob = System.IntPtr.Zero
                                self.persist = 0
                                self.attributeCount = 0
                                self.attributes = System.IntPtr.zero
                                self.targetAlias = None
                                self.userName = None


class OutObject:
    def __init__(self):
        self.arg_value = None