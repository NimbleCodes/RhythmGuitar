using SuperScrollView;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Example_Vertical
{
	[DisallowMultipleComponent]
	public sealed class Example : MonoBehaviour
	{
		[SerializeField] private LoopListView2	m_view		= null;
		[SerializeField] private SongItemDisplay m_original	= null;

		//private ListItemData[] m_list;

		private SongList songList;
		public SongItem songItem;
		public List<SongItem> items = new List<SongItem>();
		public List<NewSongItem> newItems = new List<NewSongItem>();
		public GameObject template;
		public int dirCnt;
		public SongDisplay songDisplayPrefab;

		private void Start()
		{
			/*m_list = Enumerable
				.Range( 0, 1000 )
				.Select( c => ( c + 1 ).ToString( "0000" ) )
				.Select( c => new ListItemData( c ) )
				.ToArray()
			;*/
			//foreach(SongItem item in items)
       // {
            m_view.InitListView( items.Count, OnUpdate );
       // }
			
		}

		private void LateUpdate()
		{
			m_view.UpdateAllShownItemSnapData();

			int count = m_view.ShownItemCount;

			for ( int i = 0; i < count; ++i )
			{
				var itemObj	= m_view.GetShownItemByIndex( i );
				var itemUI	= itemObj.GetComponent<ListItemUI>();
				var amount	= 1 - Mathf.Abs( itemObj.DistanceWithViewPortSnapCenter ) / 720f;
				var scale	= Mathf.Clamp( amount, 0.4f, 1 );

				itemUI.SetScale( scale );
			}
		}

		private LoopListViewItem2 OnUpdate( LoopListView2 view, int index )
		{
			if ( index < 0 || items.Count <= index ) return null;

			//var data	= items[ index ];
			var itemObj	= view.NewListViewItem( m_original.name );
			var itemUI	= itemObj.GetComponent<ListItemUI>();

			//itemUI.SetDisp( data );
			SongDisplay song = (SongDisplay)Instantiate(songDisplayPrefab);
        	song.GetComponent<Canvas>().worldCamera = Camera.main;
        	song.Prime(items);

			return itemObj;
		}
	}
}