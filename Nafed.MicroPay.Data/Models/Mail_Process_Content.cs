
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace Nafed.MicroPay.Data.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Mail_Process_Content
{

    public int ID { get; set; }

    public int MailProcessID { get; set; }

    public string Content { get; set; }

    public bool IsDeleted { get; set; }



    public virtual Mail_Process Mail_Process { get; set; }

}

}
