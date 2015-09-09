using UnityEngine;
using System;
using UnityEngine.UI;
namespace AssemblyCSharp
{
	public class Match
	{
		public string Id {
			get;
			set;
		}
		public string Revision {
			get;
			set;
		}
		public string ChallengerId {
			get;
			set;
		}
		public string ChallengedId {
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
		public string Winner {
			get;
			set;
		}
		public int Seed {
			get;
			set;
		}
		public Match ()
		{


		}
	}
}

