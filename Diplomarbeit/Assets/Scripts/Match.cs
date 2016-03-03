using UnityEngine;
using System;
using UnityEngine.UI;
namespace AssemblyCSharp
{
	public class Match
	{
		public int Id {
			get;
			set;
		}
		public int ChallengerId {
			get;
			set;
		}
		public int ChallengedId {
			get;
			set;
		}
        public string ChallengedFbId
        {
            get;
            set;
        }

		public int ChallengerScore {
			get;
			set;
		}
		public int ChallengedScore {
			get;
			set;
		}
		/*
        public string Winner {
			get;
			set;
		}
        */
		public int Seed {
			get;
			set;
		}
        public string ChallengerFbId { get; set; }

        public Match ()
		{


		}
	}
}

