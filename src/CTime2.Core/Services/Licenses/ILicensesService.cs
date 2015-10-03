using System.Collections.Generic;
using CTime2.Core.Data;

namespace CTime2.Core.Services.Licenses
{
    public interface ILicensesService
    {
        IList<License> GetLicenses();

        string GetHtmlLicenseStringFor(License license);
    }
}