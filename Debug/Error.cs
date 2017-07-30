namespace TwitchLibrary.Debug
{
    public static class Error
    {
        public static string NORMAL_NULL = "object cannot be null",
                             NORMAL_UNKNOWN = "unknown error",
                             NORMAL_FAULTED = "task has faulted",
                             NORMAL_EXCEPTION = "compiler exception",
                             NORMAL_EXISTS_NO = "object does not exist",
                             NORMAL_EXISTS_YES = "object already exists",
                             NORMAL_SYNTAX = "bad syntax",
                             NORMAL_PERMANENT = "object is permanent",
                             NORMAL_CONVERT = "could not convert the object to desired type",
                             NORMAL_OUT_OF_BOUNDS = "index out of bounds",
                             NORMAL_DESERIALIZE = "failed to deserialize the object",
                             NORMAL_SERIALIZE = "failed to serialize the object",
                             NORMAL_EQUAL = "values are equal",
                             NORMAL_MODIFIER_NO = "no modifier provided";


        public static string SYNTAX_NULL = "bad syntax, cannot be null",
                             SYNTAX_UNKNOWN = "bad syntax, unknown error",
                             SYNTAX_SQUARE_BRACKETS_NO = "bad syntax, cannot contain square brackets",
                             SYNTAX_SQUARE_BRACKETS_ENCLOSED_YES = "must be enclosed in square brackets",
                             SYNTAX_BRACKETS_NO = "bad syntax, cannot contain brackets",
                             SYNTAX_BRACKETS_ENCLOSED_YES = "bad syntax, must be enclosed in brackets",
                             SYNTAX_EQUAL_SIGN_NO = "bad syntax, cannot contain equal signs",
                             SYNTAX_SPACES_NO = "bad syntax, cannot contain spaces",
                             SYNTAX_EXCLAMATION_POINT_LEAD_YES = "bad syntax, must be lead by an exclamation point",
                             SYNTAX_LENGTH = "bad syntax, incorrect length",
                             SYNTAX_PARENTHESIS_NO = "bad syntax, cannot contain parenthesis",
                             SYNTAX_PARENTHESIS_YES = "bad syntax, must contain parenthesis",
                             SYNTAX_OUR_OF_BOUNDS = "bad syntax, index out of bounds",
                             SYNTAX_POSITIVE_YES = "bad syntax, value must be positive or zero";
    }
}
