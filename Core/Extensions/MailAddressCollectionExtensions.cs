using System.Net.Mail;

namespace RazorMail
{
    public static class MailAddressCollectionExtensions
    {
        /// <summary>
        /// Adds an email address to the end of the System.Net.Mail.MailAddressCollection
        /// </summary>
        /// <param name="source"></param>
        /// <param name="address">A System.String that contains an e-mail address.</param>
        /// <param name="displayName">A System.String that contains the display name associated with address.</param>
        public static void Add(this MailAddressCollection source, string address, string displayName)
        {
            source.Add(new MailAddress(address, displayName));
        }
    }
}
