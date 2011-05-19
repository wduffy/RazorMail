using System;
using System.IO;

namespace RazorMail.Templates
{

    internal interface ITemplate
    {
        void Write(Stream template);
    }

}
