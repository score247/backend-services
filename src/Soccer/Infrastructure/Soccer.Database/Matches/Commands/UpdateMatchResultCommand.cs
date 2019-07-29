using System;
using System.Collections.Generic;
using System.Text;
using Fanex.Data.Repository;

namespace Soccer.Database.Matches.Commands
{
    public class UpdateMatchResultCommand : NonQueryCommand
    {
        public override string GetSettingKey()
        {
            throw new NotImplementedException();
        }

        public override bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}