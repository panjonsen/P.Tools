using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P.Core.FreeswitchApi.DtoModel
{


	public record class DtoChannels
	{
		public int row_count { get; set; }

		public List<DtoChannelsData> rows { get; set; } = new();
	}

	public record class DtoChannelsData
	{
		//通话id
		public Guid uuid { get; set; }

		//方向
		public string direction { get; set; }

		//创建时间
		public string created { get; set; }

		public string name { get; set; }

		public string state { get; set; }

		public string cid_name { get; set; }

		public string cid_num { get; set; }

		public string ip_addr { get; set; }

		public string dest { get; set; }
		public string application { get; set; }

		public string application_data { get; set; }

		public string dialplan { get; set; }
		public string context { get; set; }
		public string read_codec { get; set; }

		public string read_rate { get; set; }

		public string read_bit_rate { get; set; }

		public string write_codec { get; set; }
		public string write_rate { get; set; }

		public string write_bit_rate { get; set; }

		public string secure { get; set; }

		public string hostname { get; set; }

		public string presence_id { get; set; }
		public string presence_data { get; set; }

		public string accountcode { get; set; }

		public string callstate { get; set; }

		public string callee_name { get; set; }

		public string callee_num { get; set; }

		public string callee_direction { get; set; }

		public string call_uuid { get; set; }

		public string sent_callee_name { get; set; }

		public string sent_callee_num { get; set; }

		public string initial_cid_name { get; set; }

		public string initial_cid_num { get; set; }
		public string initial_dest { get; set; }

		public string initial_dialplan { get; set; }

		public string initial_context { get; set; }
	}
}
