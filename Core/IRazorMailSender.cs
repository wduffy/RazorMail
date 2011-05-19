using System;

namespace RazorMail
{

    public interface IRazorMailSender
    {
        void Send(RazorMailMessage message);
    }

}
