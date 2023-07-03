using Nafed.MicroPay.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroPay.Web.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            List<Test> lstTest = new List<Test>()
           {
               new Test
               {
                   VM_ID=1,name="Ibrahim"
               },
               new Test
               {
                   VM_ID=2, name="Gaurav"
               }
           };
            return View("View",lstTest);
        }

        public ViewResult GetDetail(List<Test> obj)
        {
            var ids = obj.Where(x => x.IsChecked == true).Select(s => s.VM_ID).ToArray<int>();
            List<Jobs> jobs = new List<Jobs>()
           {              
               new Jobs
               {
                   VM_ID=1, Jobname="Job1"
               },
                new Jobs
               {
                   VM_ID=2,Jobname="Job2"
               }
           };

            List<Test> lstTest1 = new List<Test>()
           {
               new Test
               {
                   VM_ID=1
               },
               new Test
               {
                   VM_ID=1
               },
                new Test
               {
                   VM_ID=2
               }
           };

            var getdata = (from j1 in jobs join l1 in lstTest1 on j1.VM_ID equals l1.VM_ID where ids.Contains(l1.VM_ID) select new JobsDtl { VM_ID = l1.VM_ID, Jobname = j1.Jobname, name = l1.name }).ToList();
          
       
            return View();
        }
    }
}