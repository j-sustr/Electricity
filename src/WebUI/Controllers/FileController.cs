using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Electricity.WebUI.Controllers
{
    public class FileController : ApiController
    {
        [HttpPost]
        public Task<string> Upload(IFormFile formFile)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("{fileName}")]
        public Task<string> Delete(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}