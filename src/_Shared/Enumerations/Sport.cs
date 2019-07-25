﻿namespace Score247.Shared.Enumerations
{
    public class Sport : Enumeration
    {
        public static readonly Sport Soccer = new Sport(1, "soccer");

        public Sport()
        {
        }

        private Sport(byte value, string displayName)
            : base(value, displayName)
        {
        }
    }
}