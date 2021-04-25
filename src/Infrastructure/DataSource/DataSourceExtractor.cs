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
        private readonly IArchiveRepository _archiveRepo;

        public DataSourceExtractor(IArchiveRepository archiveRepo)
        {
            _archiveRepo = archiveRepo;
        }
        public void ExtractGroup(string groupId)
        {
            var guid = ParseGuidFromString(groupId, nameof(groupId));

            var mainArch = _archiveRepo.GetArchive(guid, 0);

            // mainArch.GetRows();
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
