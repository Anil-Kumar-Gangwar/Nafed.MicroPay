﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nafed.MicroPay.Services
{
    public static class ImageService 
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static byte[] GetImage(string imgFilePath)
        {
            log.Info($"GetImage{imgFilePath}");
            return File.ReadAllBytes(imgFilePath);
        }
    }
}
