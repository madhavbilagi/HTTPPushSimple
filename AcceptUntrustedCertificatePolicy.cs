using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace HTTPPushSimple
{
	// ms-help://MS.VSCC.2003/MS.MSDNQTR.2003FEB.1033/cpref/html/frlrfsystemneticertificatepolicyclasstopic.htm
	// The default policy is to allow valid certificates, as well as valid certificates that have expired. 
	// To change this policy, implement the ICertificatePolicy interface with a different policy, and then assign that policy to ServicePointManager.CertificatePolicy.

	public  enum    CertificateProblem  : uint
	{
		CertEXPIRED                   = 0x800B0101,
		CertVALIDITYPERIODNESTING     = 0x800B0102,
		CertROLE                      = 0x800B0103,
		CertPATHLENCONST              = 0x800B0104,
		CertCRITICAL                  = 0x800B0105,
		CertPURPOSE                   = 0x800B0106,
		CertISSUERCHAINING            = 0x800B0107,
		CertMALFORMED                 = 0x800B0108,
		CertUNTRUSTEDROOT             = 0x800B0109,
		CertCHAINING                  = 0x800B010A,
		CertREVOKED                   = 0x800B010C,
		CertUNTRUSTEDTESTROOT         = 0x800B010D,
		CertREVOCATION_FAILURE        = 0x800B010E,
		CertCN_NO_MATCH               = 0x800B010F,
		CertWRONG_USAGE               = 0x800B0110,
		CertUNTRUSTEDCA               = 0x800B0112
	}

	internal class AcceptUntrustedCertificatePolicy : ICertificatePolicy
	{
		public AcceptUntrustedCertificatePolicy()
		{
		}

		// Default policy for certificate validation.
		// public static bool DefaultValidate = false; 
  
		public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest request, int problem)
		{
			// To make sure that your programmatic SSL WebRequest accepts the SSL challenge. 
			// At least when you are encountering the Exception: "The underlying connection was closed. Could not establish trust relationship with remote server."
			// Before you call call the web request, you then set the static property CertificatePolicy on the ServicePointManager class like so:

			//	ServicePointManager.CertificatePolicy = new AcceptUntrustedCertificatePolicy();

			// it is necessary to allow for CertUNTRUSTEDROOT if the CA is not registered with the likes of VeriSign 
			if ( problem == 0 || (uint)problem == (uint)CertificateProblem.CertUNTRUSTEDROOT )
				return true;
			else
				return false;
		}

		/*
		// bug fixed version of Microsoft's example
		
		public bool CheckValidationResult(ServicePoint sp, X509Certificate cert,
			WebRequest request, int problem)
		{        
			bool ValidationResult=false;
			Console.WriteLine("Certificate Problem with accessing " +
				request.RequestUri);
			Console.Write("Problem code 0x{0:X8},",(int)problem);
			Console.WriteLine(GetProblemMessage((uint)problem));

			ValidationResult = DefaultValidate;
			return ValidationResult; 
		}

		private String GetProblemMessage(uint uiProblem)
		{
			String ProblemMessage = "";
			CertificateProblem problemList = new CertificateProblem();
			String ProblemCodeName = Enum.GetName(problemList.GetType(),uiProblem);
			if(ProblemCodeName != null)
				ProblemMessage = ProblemMessage + "-Certificateproblem:" +
					ProblemCodeName;
			else
				ProblemMessage = "Unknown Certificate Problem";
			return ProblemMessage;
		}
		*/
	}
}
