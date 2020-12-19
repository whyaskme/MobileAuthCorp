
namespace STLib
{
    public static class RequestResponseReturnCodes
    {
        public const string stapiFirstTimeAccessResponse = "INFO_PFO_00001";
        public const string stapiSuccessResponse = "INFO_PFO_00002";
        public const string stapiSuccessResponse_No_legal_for_wagering = "INFO_PFO_00041";
        public const string stapiSuccessResponse_Withdrawal_Only  = "INFO_PFO_00040";
        public const string successResponse = "INFO_PFO_00021";
        public const string ErrorResponse_System_Error_Existing_player = "ERR_PFO_00045";
        public const string ErrorResponse_Validation_Failure_Field_validation = "ERR_PFO_00001";
        public const string playerVerificationSuccessResponse = "INFO_PFO_00023";
        public const string playerRegistrationErrorResponse = "INFO_PFO_00023";
        public const string personalVerificationSuccessResponseEID = "INFO_PFO_00023";
        public const string personalVerificationErrorResponse = "ERR_PFO_00083";
        public const string personalVerificationErrorResponseEID = "ERR_PFO_00083";
        public const string stapiLoginDuringActiveSelfExclusionPeriodError = "ERR_PFO_00008";
        public const string stapiFailureToValidateOperatorError = "ERR_PFO_00178";
        public const string stapiSelfExclusionPeriodNotExtendedResponse = "INFO_PFO_00002";
        public const string stapiFailureErrorResponse = "ERR_PFO_00040";
        public const string stapiExtendSelfInclusionPeriodErrorResponse = "ERR_PFO_00008";
        public const string stapiModifyPlayerRequestValidationSuccess = "INFO_PFO_00021";
        public const string stapiModifyPlayerRequestValidateOperatorError = "ERR_PFO_00178";
    }
}
