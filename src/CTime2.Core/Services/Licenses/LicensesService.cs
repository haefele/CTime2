using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CTime2.Core.Data;

namespace CTime2.Core.Services.Licenses
{
    public class LicensesService : ILicensesService
    {
        public IList<License> GetLicenses()
        {
            return new List<License>
            {
                new License("Newtonsoft.Json"),
                new License("Caliburn.Micro"),
                new License("Microsoft HTTP Client Libraries")
            };
        }

        public string GetHtmlLicenseStringFor(License license)
        {
            string fileName = license.Name.Replace(" ", "-");

            using (var licenseStream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream($"CTime2.Core.Services.Licenses.{fileName}.lic"))
            using (var reader = new StreamReader(licenseStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}