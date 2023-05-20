using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Penfolio2.Data;

namespace Penfolio2.Areas.Identity.Pages.Account
{
    public class CustomUsernameEmailPolicy
    {
        public class CustomEmailPolicyForRegister : ValidationAttribute
        {
            protected override ValidationResult
                    IsValid(object? value, ValidationContext validationContext)
            {
                var user = (RegisterModel.InputModel)validationContext.ObjectInstance;
                List<IdentityError> errors = new List<IdentityError>();

                IsValidEmail(user.Email, checkForDuplicates: true, ref errors);

                string errorString = "";

                foreach (IdentityError error in errors)
                {
                    errorString += error.Description + " ";
                }

#pragma warning disable CS8603 // Possible null reference return.
                return errors.Count == 0 ? ValidationResult.Success : new ValidationResult(errorString);
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

        public class CustomEmailPolicyForUpdate : ValidationAttribute
        {
            protected override ValidationResult
                    IsValid(object? value, ValidationContext validationContext)
            {
                var user = (Manage.EmailModel.InputModel)validationContext.ObjectInstance;
                List<IdentityError> errors = new List<IdentityError>();

                //we're going to be checking for duplicates in the model view itself
                IsValidEmail(user.NewEmail, checkForDuplicates: false, ref errors);

                string errorString = "";

                foreach (IdentityError error in errors)
                {
                    errorString += error.Description + " ";
                }

#pragma warning disable CS8603 // Possible null reference return.
                return errors.Count == 0 ? ValidationResult.Success : new ValidationResult(errorString);
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

        public class CustomUsernamePolicy : ValidationAttribute
        {
            protected override ValidationResult
                    IsValid(object? value, ValidationContext validationContext)
            {
                var user = (RegisterModel.InputModel)validationContext.ObjectInstance;
                List<IdentityError> errors = new List<IdentityError>();

                IsValidUsername(user.UserName, ref errors);

                string errorString = "";

                foreach (IdentityError error in errors)
                {
                    errorString += error.Description + " ";
                }

#pragma warning disable CS8603 // Possible null reference return.
                return errors.Count == 0 ? ValidationResult.Success : new ValidationResult(errorString);
#pragma warning restore CS8603 // Possible null reference return.
            }
        }

        public static bool IsValidUsername(string? username, ref List<IdentityError> errors)
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
            ApplicationDbContext db = new ApplicationDbContext(options);

            if (errors == null)
            {
                errors = new List<IdentityError>();
            } //create the error list if it doesn't already exist

            int errorCount = errors.Count;

            if (username == null)
            {
                errors.Add(new IdentityError
                {
                    Description = "Please enter a username."
                });
                return false;
            } //there must be a username entered
            else if (username == "")
            {
                errors.Add(new IdentityError
                {
                    Description = "Please enter a username."
                });
                return false;
            } //the username cannot be an empty string
            else
            {
                if ((!username.StartsWith('@') && username.Contains('@'))
                || (username.StartsWith('@') && username.IndexOf('@') != username.LastIndexOf('@')))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "In order to keep usernames from resembling email addresses, the '@' symbol is only allowed at the beginning of usernames."
                    });
                } //if the username contains an '@' symbol anywhere but the beginning of the username

                if (username.StartsWith(' ') || username.EndsWith(' '))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "Spaces are only allowed in the middle of a username, not at the beginning or ending."
                    });
                } //if there's a space at the beginning or ending of the username

                //get a char array of the username entered
                char[] chars = username.ToUpper().ToCharArray();

                foreach (char c in chars)
                {
                    switch (c)
                    {
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                        case 'G':
                        case 'H':
                        case 'I':
                        case 'J':
                        case 'K':
                        case 'L':
                        case 'M':
                        case 'N':
                        case 'O':
                        case 'P':
                        case 'Q':
                        case 'R':
                        case 'S':
                        case 'T':
                        case 'U':
                        case 'V':
                        case 'W':
                        case 'X':
                        case 'Y':
                        case 'Z':
                        case '!':
                        case '#':
                        case '$':
                        case '%':
                        case '&':
                        case '\'':
                        case '*':
                        case '+':
                        case '-':
                        case '/':
                        case '=':
                        case '?':
                        case '^':
                        case '_':
                        case '`':
                        case '{':
                        case '|':
                        case '}':
                        case '~':
                        case '.':
                        case ' ':
                        case '\"':
                        case '(':
                        case ')':
                        case ',':
                        case ':':
                        case ';':
                        case '<':
                        case '>':
                        case '@':
                        case '[':
                        case ']':
                        case '\\':
                            break;
                        default:
                            errors.Add(new IdentityError
                            {
                                Description = "Usernames can only include uppercase or lowercase Latin letters A to Z and a to z, the digits 0 to 9, spaces in the middle of the username, the '@' symbol if it is the first character of the username, or the printable characters .!#$%&'*+-/=?^_`{|}~\"(),:;<>[\\]."
                            });
                            return false;
                    }
                } //check each character to make sure they are allowed

                foreach (string? u in db.Users.Select(u => u.NormalizedUserName))
                {
                    if (u != null)
                    {
                        if (username.ToUpper() == u)
                        {
                            errors.Add(new IdentityError
                            {
                                Description = "There is already an account with this username."
                            });
                            return false;
                        }
                    }
                } //check against the users already in the database to make sure it's not a duplicate
            } //if there was a username entered, check the other requirements

            return errors.Count == errorCount ? true : false;
        }

        public static bool IsValidEmail(string? email, bool checkForDuplicates, ref List<IdentityError> errors)
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptions<ApplicationDbContext>();
            ApplicationDbContext db = new ApplicationDbContext(options);
            bool isValidLocalPart = false;
            bool isValidDomain = false;

            if (email == null)
            {
                errors.Add(new IdentityError
                {
                    Description = "Please enter an email address."
                });
                return false;
            } //no email address has been entered
            else if (!email.Contains('@'))
            {
                errors.Add(new IdentityError
                {
                    Description = "A valid email address must contain a local-part and a domain separated by the '@' symbol."
                });
                return false;
            } //there is no @ symbol in the email address
            else
            {
                string[] emailParts = email.Split('@');

                if (emailParts.Length < 2)
                {
                    errors.Add(new IdentityError
                    {
                        Description = "A valid email address must consist of a local-part, the '@' symbol, and a domain."
                    });
                    return false;
                } //the email address was "@" or ended or began with the @ symbol
                else
                {
                    string localPart = "";
                    string domain = emailParts[emailParts.Length - 1];

                    for (int i = 0; i < emailParts.Length - 1; i++)
                    {
                        localPart += emailParts[i];

                        if (i < emailParts.Length - 2)
                        {
                            localPart += "@";
                        } //will only add a @ if there was more than one @ in the email address
                    } //create the local-part

                    //check if the local-part is valid
                    isValidLocalPart = IsValidLocalPart(localPart, ref errors);

                    //check if the domain is valid 
                    isValidDomain = IsValidDomain(domain, ref errors);

                    if (checkForDuplicates)
                    {
                        foreach (string? e in db.Users.Select(e => e.NormalizedEmail))
                        {
                            if (e != null)
                            {
                                if (NormalizeEmail(email) == e)
                                {
                                    errors.Add(new IdentityError
                                    {
                                        Description = "There is already an account with this email address."
                                    });
                                    return false;
                                }
                            }
                        } //check against the emails already in the database to make sure it's not a duplicate
                    } //if we want to check to make sure it's not already in the database
                } //if the email address has a local-part and a domain separated by an @, create the local-part and the domain, and check if they are both valid
            } //if there's an @ in the email address 

            return isValidLocalPart && isValidDomain;
        }

        protected static bool IsValidLocalPart(string localPart, ref List<IdentityError> errors)
        {
            if (errors is null)
            {
                errors = new List<IdentityError>();
            } //if errors is null, create a new list of IdentityErrors

            int errorCount = errors.Count;

            if (localPart.Length > 64)
            {
                errors.Add(new IdentityError
                {
                    Description = "The local-part of the email address cannot be longer than 64 characters."
                });
            } //if the local part is too long

            if (localPart.StartsWith('\"') && localPart.EndsWith('\"'))
            {
                if (localPart.Length > 2)
                {
                    string insideQuotes = localPart.Substring(1, localPart.Length - 2);

                    if (insideQuotes.EndsWith('\\'))
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "The final '\"' symbol of an email address's local-part cannot be an escaped character."
                        });
                    } //if the final quote has been escaped 

                    if (insideQuotes.Contains('\"'))
                    {
                        string[] byQuotes = insideQuotes.Split('\"');

                        for (int i = 0; i < byQuotes.Length - 1; i++)
                        {
                            if (!byQuotes[i].EndsWith('\\'))
                            {
                                errors.Add(new IdentityError
                                {
                                    Description = "In an email address's local-part that has been enclosed in quotes, all further '\"' symbols must be proceeded by a '\\' symbol."
                                });

                                break;
                            } //if there's a " in the middle of the quoted text that's not escaped
                        }
                    } //if there's a " in the middle of the quoted text

                    if (insideQuotes.Contains('\\'))
                    {
                        string[] byQuotes = insideQuotes.Split("\\\\");
                        //"just\"not\\"rig\\\ht"@example.com
                        //\"just\\\"not\\\\\"rig\\\\\\ht\"@example.com
                        //0: just\\\"not
                        //1: \"rig
                        //2: \\ht

                        for (int i = 0; i < byQuotes.Length; i++)
                        {
                            if (byQuotes[i].StartsWith('\"'))
                            {
                                errors.Add(new IdentityError
                                {
                                    Description = "In an email address's local-part that has been enclosed in quotes, all '\"' symbols must be preceded by a '\\' symbol that hasn't already been escaped by a '\\' symbol."
                                });
                                break;
                            } //if the segment starts with a "
                            else if (byQuotes[i].Contains('\\'))
                            {
                                //if there's a backslash in this section, create a char array so we can figure this out
                                char[] substring = byQuotes[i].ToCharArray();

                                for (int j = 0; j < substring.Length; j++)
                                {
                                    if (substring[j] == '\\')
                                    {
                                        if (j + 1 < substring.Length)
                                        {
                                            if (substring[j + 1] != '\"')
                                            {
                                                errors.Add(new IdentityError
                                                {
                                                    Description = "In an email address's local-part that has been enclosed in quotes, all '\\' symbols must be proceeded by a '\\' symbol."
                                                });
                                                break;
                                            } //if the backslash is not escaping a ", it's an invalid character
                                        } //if there are other characters after this one, we want to check and see if the next one is a "
                                        else
                                        {
                                            errors.Add(new IdentityError
                                            {
                                                Description = "In an email address's local-part that has been enclosed in quotes, all '\\' symbols must be proceeded by a '\\' symbol."
                                            });
                                            break;
                                        } //if there aren't characters after the backslash, it can't be escaping a "
                                    } //if this is the index of the backslash
                                } //check all of the characters in the array
                            } //if there's still a backslash after we have gotten rid of all of the escaped backslashes
                        } //check each substring to see if they still have a backslash after we have gotten rid of all of the escaped backslashes
                    } //if there's a backslash inside the quote
                } //if there's actually something inside the quotes, get that
            } //there are different rules for email addresses surrounded by quotes, so let's look at those first
            else
            {
                if (localPart.Contains(".."))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain two '.' symbols in a row."
                    });
                } //if the local-part has multiple periods in a row outside of quotes

                if (localPart.StartsWith("."))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot start with a period."
                    });
                } //if the local-part starts with a period

                if (localPart.EndsWith("."))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot end with a period."
                    });
                } //if the local-part ends with a period

                if (localPart.Contains("@"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "An email address cannot contain more than one '@' symbol outside of quotations."
                    });
                } //if the local-part contains a @

                if (localPart.Contains(","))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain a ',' symbol outside of quotations."
                    });
                } //if the local-part contains a ,

                if (localPart.Contains(":"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain a ':' symbol outside of quotations."
                    });
                } //if the local-part contains a :

                if (localPart.Contains(";"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain a ';' symbol outside of quotations."
                    });
                } //if the local-part contains a ;

                if (localPart.Contains("<"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain a '<' symbol outside of quotations."
                    });
                } //if the local-part contains a <

                if (localPart.Contains(">"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain a '>' symbol outside of quotations."
                    });
                } //if the local-part contains a >

                if (localPart.Contains("["))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain a '[' symbol outside of quotations."
                    });
                } //if the local-part contains a [

                if (localPart.Contains("]"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain a ']' symbol outside of quotations."
                    });
                } //if the local-part contains a ]

                if (localPart.Contains("\\"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain a '\\' symbol outside of quotations."
                    });
                } //if the local-part contains a \

                if (localPart.Contains("\""))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain a '\"' symbol outside of quotations."
                    });
                } //if the local-part contains a "

                if (localPart.Contains(" "))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain a space outside of quotations."
                    });
                } //if the local-part contains a space

                if (localPart.Contains("(") && !localPart.Contains(")"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "A comment in the local-part of an email address must be fully closed by parentheses."
                    });
                } //if there's an opening parenthesis but not a closing one

                if (localPart.Contains(")") && !localPart.Contains("("))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "A comment in the local-part of an email address must be fully closed by parentheses."
                    });
                } //if there's a closing parenthesis but not an opening one

                if (!localPart.StartsWith("(") && localPart.Contains("(") && localPart.Contains(")") && !localPart.EndsWith(")"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "A comment in the local-part of an email address must be fully closed by parentheses and is only allowed at the beginning or end of the local-part."
                    });
                } //if there's a comment in the middle of the local-part

                if (localPart.StartsWith("(") && localPart.EndsWith(")"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address must contain more than a comment."
                    });
                } //if there's nothing before or after the comment

                if (localPart.IndexOf("(") != localPart.LastIndexOf("("))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain more than one '(' symbol outside of quotations."
                    });
                } //if there's more than one (

                if (localPart.IndexOf(")") != localPart.LastIndexOf(")"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The local-part of an email address cannot contain more than one ')' symbol outside of quotations."
                    });
                } //if there's more than one )
            } //there are different rules for local-parts that are surrounded by quotes, so let's only look at local-parts where this is not true

            return errors.Count == errorCount;
        }

        protected static bool IsValidDomain(string domain, ref List<IdentityError> errors)
        {

            if (errors is null)
            {
                errors = new List<IdentityError>();
            }

            int errorCount = errors.Count;

            if (IsValidIPAddress(domain, ref errors))
            {
                return true;
            }
            else
            {
                if (domain.StartsWith("-"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The domain of an email address cannot start with a '-' symbol."
                    });
                } //if the domain starts with a -

                if (domain.EndsWith("-"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The domain of an email address cannot end with a '-' symbol."
                    });
                } //if the domain ends with a -

                if (domain.Contains("(") && !domain.Contains(")"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "A '(' symbol in the domain of an email address must always be paired with a ')' symbol."
                    });
                } //if there's an opening parenthesis but no closing parenthesis

                if (domain.Contains(")") && !domain.Contains("("))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "A ')' symbol in the domain of an email address must always be proceeded by a '(' symbol."
                    });
                } //if there's a closing parenthesis but no opening parenthesis

                if (!domain.StartsWith("(") && domain.Contains("(") && !domain.EndsWith(")") && domain.Contains(")"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "A comment surrounded by parentheses in the domain of an email address must appear at either the beginning or end of the domain and cannot appear in the middle."
                    });
                } //if there's a comment in the middle of the domain

                if (domain.IndexOf("(") != domain.LastIndexOf("("))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The domain of an email address cannot have more than one '(' symbol."
                    });
                } //if there's more than one ( in the domain

                if (domain.IndexOf(")") != domain.LastIndexOf(")"))
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The domain of an email address cannot have more than one ')' symbol."
                    });
                } //if there's more than one ) in the domain

                //turn the domain into a char array
                char[] chars = domain.ToCharArray();

                foreach (char c in chars)
                {
                    switch (c)
                    {
                        case '[':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot a '[' symbol unless it's part of a pair of square brackets surrounding an IP literal."
                            });
                            break;
                        case ']':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot a ']' symbol unless it's part of a pair of square brackets surrounding an IP literal."
                            });
                            break;
                        case '\\':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '\\' symbol."
                            });
                            break;
                        case '\"':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '\"' symbol."
                            });
                            break;
                        case '>':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '>' symbol."
                            });
                            break;
                        case '<':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '<' symbol."
                            });
                            break;
                        case '~':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '~' symbol."
                            });
                            break;
                        case '{':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '{' symbol."
                            });
                            break;
                        case '}':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '}' symbol."
                            });
                            break;
                        case '|':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '|' symbol."
                            });
                            break;
                        case '`':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '`' symbol."
                            });
                            break;
                        case '_':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain an '_' symbol."
                            });
                            break;
                        case '^':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '^' symbol."
                            });
                            break;
                        case '?':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '?' symbol."
                            });
                            break;
                        case '=':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain an '=' symbol."
                            });
                            break;
                        case '/':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '/' symbol."
                            });
                            break;
                        case '+':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '+' symbol."
                            });
                            break;
                        case '*':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain an '*' symbol."
                            });
                            break;
                        case '\'':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a \"'\" symbol."
                            });
                            break;
                        case '&':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain an '&' symbol."
                            });
                            break;
                        case '%':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '%' symbol."
                            });
                            break;
                        case '#':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '#' symbol."
                            });
                            break;
                        case '!':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a '!' symbol."
                            });
                            break;
                        case ';':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a ';' symbol."
                            });
                            break;
                        case ',':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain a ',' symbol."
                            });
                            break;
                        case ' ':
                            errors.Add(new IdentityError
                            {
                                Description = "The domain of an email address cannot contain spaces."
                            });
                            break;
                        default:
                            break;
                    } //switch statement to add errors for the different characters that aren't allowed
                } //check each character in the domain
            }
            return errors.Count == errorCount;
        }

        protected static bool IsValidIPAddress(string domain, ref List<IdentityError> errors)
        {
            if (!domain.StartsWith(']') && !domain.EndsWith(']'))
            {
                return false;
            } //can't be an IP address if it isn't surrounded by brackets
            else if (!domain.Contains(':') && !domain.Contains('.'))
            {
                return false;
            } //needs to contain : or . to be an IP adddress
            else if (domain.Contains(':'))
            {
                return IsValidIPv6(domain, ref errors);
            } //if it contains a :, check if it's a valid IPv6 or IPv6 Dual
            else
            {
                return IsValidIPv4(domain, ref errors);
            } //if it contains a . and not an :, check if it's an IPv4
        }

        protected static bool IsValidIPv4(string domain, ref List<IdentityError> errors)
        {
            string insideBrackets = domain.Substring(1, domain.Length - 2);
            string[] potentialIPv4 = insideBrackets.Split('.');

            errors = errors is null ? new List<IdentityError>() : errors;

            if (potentialIPv4.Length != 4)
            {
                errors.Add(new IdentityError
                {
                    Description = "An IPv4 address in the domain of an email address must have exactly 4 parts."
                });

                return false;
            } //if what's inside the brackets doesn't have the correct number of segments to be an IPv4

            foreach (string segment in potentialIPv4)
            {
                if (Int32.TryParse(segment, out int numOut))
                {
                    if (numOut > 255 || numOut < 0)
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "The IPv4 address surrounded by brackets in the domain of the email address must be made up of 4 numbers between 0 and 255 separated by '.' symbols."
                        });
                        return false;
                    }
                } //if the part is a number, make sure it's in the right range
                else
                {
                    errors.Add(new IdentityError
                    {
                        Description = "The IPv4 address surrounded by brackets in the domain of the email address must be made up of 4 numbers between 0 and 255 separated by '.' symbols."
                    });
                    return false;
                } //if one of the parts of the IPv4 isn't a number
            } //check to make sure that each part of the IPv4 is valid

            return true;
        } //protected static bool IsValidIPv4(string domain, ref List<IdentityError> errors)

        protected static bool IsValidIPv6(string domain, ref List<IdentityError> errors)
        {
            domain = domain.ToUpper();
            string insideBrackets = domain.Substring(1, domain.Length - 2);
            string[] potentialIPv6 = insideBrackets.Split(':');


            if (potentialIPv6.Length < 3)
            {
                errors.Add(new IdentityError
                {
                    Description = "The IPv6 address surrounded by brackets in the domain of the email address doesn't have enough segments separated by ':' symbols."
                });
                return false;
            } //if the IPv6 doesn't have enough segments
            else if (potentialIPv6.Length > 9
                || (potentialIPv6.Length == 9 && potentialIPv6[0] != "IPV6" && !potentialIPv6[8].Contains('.'))
                || (potentialIPv6.Length == 10 && potentialIPv6[0] != "IPV6" && !potentialIPv6[9].Contains('.')))
            {
                errors.Add(new IdentityError
                {
                    Description = "The IPv6 address surrounded by brackets in the domain of the email address features too many segments separated by ':' symbols."
                });
                return false;
            } //if the IPv6 has too many segments

            int firstValidSegment = 0;
            bool isIPv6Dual = false;

            if (potentialIPv6[0] == "IPV6")
            {
                firstValidSegment = 1;
            } //if the first segment is IPv6, we only check the segments after that

            if (potentialIPv6[potentialIPv6.Length - 1].Contains('.'))
            {
                isIPv6Dual = true;
            }

            for (int i = firstValidSegment; i < potentialIPv6.Length; i++)
            {
                if (i == potentialIPv6.Length - 1 && isIPv6Dual)
                {
                    string[] potentialIPv4 = potentialIPv6[i].Split('.');

                    if (potentialIPv4.Length != 4)
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "The IPv6 Dual address surrounded by brackets in the domain of the email address does not have an IPv4 address of the right length at the end."
                        });
                        return false;
                    } //if the ipv4 part isn't the correct length
                    else
                    {
                        foreach (string segment in potentialIPv4)
                        {
                            if (Int32.TryParse(segment, out int numOut))
                            {
                                if (numOut > 255 || numOut < 0)
                                {
                                    errors.Add(new IdentityError
                                    {
                                        Description = "The IPv4 address at the end of the IPv6 Dual address surrounded by brackets in the domain of the email address must be made up of 4 numbers between 0 and 255 separated by '.' symbols."
                                    });
                                    return false;
                                }
                            } //if the part is a number, make sure it's in the right range
                            else
                            {
                                errors.Add(new IdentityError
                                {
                                    Description = "The IPv4 address at the end of the IPv6 Dual address surrounded by brackets in the domain of the email address must be made up of 4 numbers between 0 and 255 separated by '.' symbols."
                                });
                                return false;
                            } //if one of the parts of the IPv4 isn't a number
                        } //check to make sure that each part of the IPv4 is valid
                    } //if this is the last segment of an IPv6 Dual, we need to make sure this is a valid IPv4
                } //if it's an IPv6 Dual and we are looking at the IPv4 part
                else
                {
                    if (potentialIPv6[i].Length > 4)
                    {
                        errors.Add(new IdentityError
                        {
                            Description = "The IPv6 address surrounded by brackets in the domain of the email address cannot have any segments separated by ':' symbols which are made up of greater than 4 characters."
                        });
                        return false;
                    } //if a segment has too many characters

                    foreach (char c in potentialIPv6[i])
                    {
                        if (!(c >= '0' && c <= '9') && !(c >= 'A' && c <= 'F'))
                        {
                            errors.Add(new IdentityError
                            {
                                Description = "The IPv6 address surrounded by brackets in the domain of the email address must be made of up segments in hexadecimal format."
                            });
                            return false;
                        } //if a segment has an invalid character
                    } //check all of the characters in the current segment
                } //if we aren't currently looking at an IPv4
            } //check all of the segments in the domain

            return true;
        } //protected bool IsValidIPv6(string domain, string email, ref List<IdentityError> errors)

        public static string NormalizeEmail(string email)
        {
            string normalizedLocalPart = "";
            string normalizedDomain = "";

            if (email is null)
            {
                return "";
            } //if it's null, return an empty string
            else
            {
                email = email.ToUpper();

                if (!email.Contains('@'))
                {
                    return email;
                } //if it's not a valid email address, return the to upper version
                else
                {
                    string[] emailParts = email.Split('@');

                    if (emailParts.Length < 2)
                    {
                        return email;
                    } //if it doesn't have enough parts, return the to upper version of the email

                    string domain = emailParts[emailParts.Length - 1];
                    string localPart = "";

                    for (int i = 0; i < emailParts.Length - 1; i++)
                    {
                        localPart += emailParts[i];

                        if (i < emailParts.Length - 2)
                        {
                            localPart += "@";
                        } //will only add a @ if there was more than one @ in the email address
                    } //create the local-part

                    if (localPart.StartsWith('(') && localPart.Contains(')') && !localPart.EndsWith(')') && localPart.IndexOf('(') == localPart.LastIndexOf('(') && localPart.IndexOf(')') == localPart.LastIndexOf(')'))
                    {
                        string[] byComment = localPart.Split(')');

                        normalizedLocalPart = byComment[1];
                    } //if the local-part begins with a proper comment that follows the rules, get rid of the comment as part of normalization
                    else if (!localPart.StartsWith('(') && localPart.Contains('(') && localPart.EndsWith(')') && localPart.IndexOf('(') == localPart.LastIndexOf('(') && localPart.IndexOf(')') == localPart.LastIndexOf(')'))
                    {
                        string[] byComment = localPart.Split('(');

                        normalizedLocalPart = byComment[0];
                    } //if the local-part ends with a proper comment that follows the rules, get rid of the comment as part of normalization
                    else
                    {
                        normalizedLocalPart = localPart;
                    } //otherwise, the normalized local-part is just the local-part in uppercase

                    if (domain.StartsWith('[') && domain.EndsWith(']') && domain.Contains(':'))
                    {
                        normalizedDomain = NormalizeIPv6(domain);
                    } //if it's a potential IPv6 or IPv6 Dual, call NormalizeIPv6
                    else if (domain.StartsWith('(') && domain.Contains(')') && !domain.EndsWith(')') && domain.IndexOf('(') == domain.LastIndexOf('(') && domain.IndexOf(')') == domain.LastIndexOf(')'))
                    {
                        string[] byComment = domain.Split(')');

                        normalizedDomain = byComment[1];
                    } //if the domain starts with a proper comment that follows the rules, get rid of the comment as part of normalization
                    else if (!domain.StartsWith('(') && domain.Contains('(') && domain.EndsWith(')') && domain.IndexOf('(') == domain.LastIndexOf('(') && domain.IndexOf(')') == domain.LastIndexOf(')'))
                    {
                        string[] byComment = domain.Split('(');

                        normalizedDomain = byComment[0];
                    } //if the domain ends with a proper comment that follows the rules, get rid of the comment as part of normalization
                    else
                    {
                        normalizedDomain = domain;
                    } //otherwise, the normalized domain is just the domain in uppercase
                } //if it has a @
            } //if email isn't null

            return normalizedLocalPart + "@" + normalizedDomain;
        }

        protected static string NormalizeIPv6(string domain)
        {
            //put everything in uppercase
            domain = domain.ToUpper();

            if (!domain.StartsWith('[') && !domain.EndsWith(']'))
            {
                return domain;
            } //if it isn't surrounded by brackets, it's not an IP address, so just return the domain as it is

            string insideBrackets = domain.Substring(1, domain.Length - 2);
            string normalizedIPv6Builder = "[";

            if (!insideBrackets.Contains(':'))
            {
                return domain;
            } //if there are no :, it can't be an IPv6 or an IPv6 Dual, so return the domain as it is

            string[] currentIPv6 = insideBrackets.Split(':');

            if (currentIPv6.Length < 3)
            {
                return domain;
            } //if it's too short, we know it's not an IPv6 or an IPv6 Dual, so return the domain as it is

            int firstValidSegment = 0;

            if (currentIPv6[0] == "IPV6")
            {
                firstValidSegment = 1;
            } //we don't care about the first segment if it's IPV6

            bool isIPv6Dual = false;
            int lastEmptyString = -1;

            if (currentIPv6[currentIPv6.Length - 1].Contains('.'))
            {
                isIPv6Dual = true;
            }

            if ((isIPv6Dual && currentIPv6.Length > 9 + firstValidSegment) || (!isIPv6Dual && currentIPv6.Length > 8 + firstValidSegment))
            {
                return domain;
            } //if it has too many segments, it can't be an IPv6, so just return the domain as it is

            for (int i = firstValidSegment; i < currentIPv6.Length; i++)
            {
                int indexOfLastLeadingZero = -1;
                string normalizedSegment = "";

                if (currentIPv6[i].Length > 4 && !(i == currentIPv6.Length - 1 && isIPv6Dual))
                {
                    return domain;
                } //if the segment is too long, we know it's not an IPv6, so just return the domain as is

                char[] chars = currentIPv6[i].ToCharArray();

                for (int j = 0; j < chars.Length; j++)
                {
                    if (!(chars[j] >= '0' && chars[j] <= '9') && !(chars[j] >= 'A' && chars[j] <= 'F'))
                    {
                        return domain;
                    } //if there's an invalid character, we know it's not an IPv6, so just return the domain as it is

                    if (j == 0)
                    {
                        if (chars[j] == '0')
                        {
                            indexOfLastLeadingZero = j;
                        } //if it's 0, update the index of the last leading 0
                        else
                        {
                            normalizedSegment += chars[j];
                        } //otherwise, add the character to the normalized segment
                    } //if this is the first character in the segment
                    else if (j - 1 == indexOfLastLeadingZero)
                    {
                        if (chars[j] == '0')
                        {
                            indexOfLastLeadingZero = j;
                        } //if the current character is another leading 0, update the index of the last leading 0
                        else
                        {
                            normalizedSegment += chars[j];
                        } //otherwise, add the character to the normalized segment
                    } //if the previous character was a leading 0
                    else
                    {
                        normalizedSegment += chars[j];
                    } //otherwise, add the character to the normalized segment
                } //check all of the characters in the current segment

                if (normalizedSegment == "")
                {
                    if (i == firstValidSegment || i == currentIPv6.Length - 1)
                    {
                        normalizedIPv6Builder += ":";
                    } //if the first or last segment is an empty string, it needs a : to represent that
                    //update the index of the last empty string
                    lastEmptyString = i;
                } //if the normalized segment is still an empty string
                else if (isIPv6Dual && i == currentIPv6.Length - 1)
                {
                    normalizedIPv6Builder += ":";
                    normalizedIPv6Builder += currentIPv6[i];
                } //if it's the last segment of an IPv6 Dual
                else
                {
                    if (i == firstValidSegment)
                    {
                        normalizedIPv6Builder += normalizedSegment;
                    } //if it's the first segment, we don't have to worry about leading :
                    else if (firstValidSegment == i - 1 && lastEmptyString == i - 1)
                    {
                        normalizedIPv6Builder += normalizedSegment;
                    } //if this is the second segment and the first segment was an empty string, we should already have a leading : and don't need to add one
                    else if (lastEmptyString == i - 1)
                    {
                        normalizedIPv6Builder += "::";
                        normalizedIPv6Builder += normalizedSegment;
                    } //if the previous segment was an empty string and it wasn't the first segment, we need two leading : to represent the gap
                    else
                    {
                        normalizedIPv6Builder += ":";
                        normalizedIPv6Builder += normalizedSegment;
                    } //otherwise, we only need the one leading :
                } //if it's not the last segment of an IPv6 Dual and the normalized segment isn't an empty string

            } //check all of the segments in the IPv6

            normalizedIPv6Builder += "]";

            return normalizedIPv6Builder;
        }
    }
}
