using Microservice.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microservice.Framework.Domain.Extensions
{
    public enum AllowedCharacter
    {
        Alpha,
        Ampersand,
        Apostrophe,
        Asterisk,
        Backslash,
        Colon,
        Comma,
        Dot,
        Dash,
        EnDash,
        EmDash,
        Hyphen,
        Equals,
        ForwardSlash,
        Hash,
        Numeric,
        Percentage,
        Plus,
        RoundBrackets,
        Semicolon,
        Space,
        Exclamation,
        QuestionMark,
        AccentedVowelsUpper,
        AccentedVowelsLower
    }

    public static class StringValidationHelper
    {
        #region Methods

        private static string ConvertToRegularExpression(AllowedCharacter allowedCharacter)
        {
            var regExpression = string.Empty;

            switch (allowedCharacter)
            {
                case AllowedCharacter.Ampersand:
                    regExpression = @"[&]";
                    break;
                case AllowedCharacter.Apostrophe:
                    regExpression = @"[']";
                    break;
                case AllowedCharacter.Alpha:
                    regExpression = @"[a-zA-Z]";
                    break;
                case AllowedCharacter.Asterisk:
                    regExpression = @"[\*]";
                    break;
                case AllowedCharacter.Backslash:
                    regExpression = @"[\\]";
                    break;
                case AllowedCharacter.Colon:
                    regExpression = @"[:]";
                    break;
                case AllowedCharacter.ForwardSlash:
                    regExpression = @"[\/]";
                    break;
                case AllowedCharacter.Comma:
                    regExpression = @"[,]";
                    break;
                case AllowedCharacter.Equals:
                    regExpression = @"[\=]";
                    break;
                case AllowedCharacter.Percentage:
                    regExpression = @"[\%]";
                    break;
                case AllowedCharacter.Semicolon:
                    regExpression = @"[;]";
                    break;
                case AllowedCharacter.Dot:
                    regExpression = @"[.]";
                    break;
                case AllowedCharacter.Dash:
                    regExpression = @"[\-]";
                    break;
                case AllowedCharacter.EnDash:
                    regExpression = @"[\–]";
                    break;
                case AllowedCharacter.EmDash:
                    regExpression = @"[\—]";
                    break;
                case AllowedCharacter.Hash:
                    regExpression = @"[\#]";
                    break;
                case AllowedCharacter.Plus:
                    regExpression = @"[\+]";
                    break;
                case AllowedCharacter.RoundBrackets:
                    regExpression = @"[\(\)]";
                    break;
                case AllowedCharacter.Space:
                    regExpression = @"[\s]";
                    break;
                case AllowedCharacter.Numeric:
                    regExpression = @"[0-9]";
                    break;
                case AllowedCharacter.Exclamation:
                    regExpression = @"[!]";
                    break;
                case AllowedCharacter.QuestionMark:
                    regExpression = @"[?]";
                    break;
                case AllowedCharacter.AccentedVowelsUpper:
                    regExpression = @"[ÀÁÂÄÈÉÊËÌÍÎÏÕÒÓÔÖÙÚÛÜÝ]";
                    break;
                case AllowedCharacter.AccentedVowelsLower:
                    regExpression = @"[àáâäèéêëìíîïõòóôöùúûüý]";
                    break;
                default:
                    break;
            }

            return regExpression;
        }

        public static bool ValidateString(string valueToCheck, IList<AllowedCharacter> allowedCharacters, out string invalidCharacters)
        {
            if (string.IsNullOrEmpty(valueToCheck))
            {
                invalidCharacters = "Value required";
                return false;
            }

            var formattedValue = valueToCheck;

            foreach (AllowedCharacter type in allowedCharacters)
            {
                formattedValue = Regex.Replace(formattedValue, ConvertToRegularExpression(type), string.Empty, RegexOptions.IgnoreCase);
            }

            invalidCharacters = formattedValue;
            return invalidCharacters.IsNullOrEmpty();
        }

        public static bool ValidateString(string valueToCheck, params AllowedCharacter[] allowedCharacters)
        {
            var invalidCharacters = string.Empty;
            return ValidateString(valueToCheck, allowedCharacters, out invalidCharacters);
        }

        public static bool ContainsString(string valueToCheck, params AllowedCharacter[] characters)
        {
            foreach (AllowedCharacter type in characters)
            {
                if (Regex.Match(valueToCheck, ConvertToRegularExpression(type), RegexOptions.IgnoreCase).Success)
                {
                    return true;
                }
            }

            return false;
        }

        public static string StripCharacters(string originalValue, params AllowedCharacter[] allowedCharacters)
        {
            Invariant.ArgumentNotEmpty(originalValue, () => "OriginalValue");
            var formattedValue = originalValue;

            foreach (int value in Enum.GetValues(typeof(AllowedCharacter)))
            {
                var allowedCharacter = (AllowedCharacter)value;
                if (!allowedCharacters.Contains(allowedCharacter))
                {
                    formattedValue = Regex.Replace(formattedValue, ConvertToRegularExpression(allowedCharacter), string.Empty, RegexOptions.IgnoreCase);
                }
            }
            return formattedValue;
        }

        public static bool StringContainsNumerics(string value)
        {
            var allowedCharacters = new List<AllowedCharacter>() { AllowedCharacter.Numeric }.ToArray();

            return StripCharacters(value, allowedCharacters).IsNotNullOrEmpty();
        }

        public static bool StringContainsOnlyNumerics(string value)
        {
            return Regex.IsMatch(value, @"^\d+$");
        }

        public static bool StringContainsSpace(string value)
        {
            return Regex.IsMatch(value, @"\s");
        }
        public static bool StringIsValidLength(string value, int minimumLength, int maximumLength)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length >= minimumLength && value.Length <= maximumLength)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ValidateEmailAddress(string emailAddress)
        {
            emailAddress = emailAddress.Trim();
            if (emailAddress.IsNullOrEmpty() || StringContainsSpace(emailAddress))
            {
                return false;
            }

            try
            {
                MailAddress mailAddress = new MailAddress(emailAddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool ValidateUrl(string webAddress)
        {
            string http = @"http://";
            if (!string.IsNullOrEmpty(webAddress))
            {
                if (webAddress.StartsWith(http, StringComparison.CurrentCulture))
                {
                    var webAddressExpression = @"^(([\w]+:)?\/\/)?(([\d\w]|%[a-fA-f\d]{2,2})+(:([\d\w]|%[a-fA-f\d]{2,2})+)?@)?([\d\w][-\d\w]{0,253}[\d\w]\.)+[\w]{2,4}(:[\d]+)?(\/([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)*(\?(&?([-+_~.\d\w]|%[a-fA-f\d]{2,2})=?)*)?(#([-+_~.\d\w]|%[a-fA-f\d]{2,2})*)?$";

                    return Regex.Match(webAddress, webAddressExpression).Success;
                }
            }
            return false;
        }

        #endregion
    }
}
