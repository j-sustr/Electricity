using Electricity.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electricity.Infrastructure.DataSource
{
    public class DataSourceExtractor
    {
        private readonly string[] _mainQuantities = new string[] {
            "P_avg_3P",
            "P_avg_P1",
            "P_avg_P2",
            "P_avg_P3",
        };

        private readonly string[] emQuantities = new string[] {
            
        };

        private readonly IArchiveRepository _archiveRepo;

        public DataSourceExtractor(IArchiveRepository archiveRepo)
        {
            _archiveRepo = archiveRepo;
        }
        public void ExtractGroup(string groupId)
        {
            var guid = ParseGuidFromString(groupId, nameof(groupId));

            var mainArch = _archiveRepo.GetArchive(guid, 0);

            mainArch.GetRows(new GetArchiveRowsQuery { 
            
            });
        }

        private Guid ParseGuidFromString(string id, string name)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                throw new ArgumentException($"could not parse Guid", name);
            }
            return guid;
        }
    }
}
