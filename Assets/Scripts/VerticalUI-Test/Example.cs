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

		//private SongItem[] m_list;
		public List<SongItem> m_list;

		//EDITOR에서 초기화 필요
		public SongList songList;
		public SongItem songItem;
		public List<SongItem> items = new List<SongItem>();
		public List<NewSongItem> newItems = new List<NewSongItem>();
		public GameObject template;
		public int dirCnt;
		public SongDisplay songDisplayPrefab;

		private void Start()
		{
			m_list = new List<SongItem>();
			songList.items.ForEach((item)=>{
				m_list.Add(item);
			});
			m_view.InitListView( m_list.Count, OnUpdate );
		}

		private void LateUpdate()
		{
			m_view.UpdateAllShownItemSnapData();

			int count = m_view.ShownItemCount;

			for ( int i = 0; i < count; ++i )
			{
				var itemObj	= m_view.GetShownItemByIndex( i );
				var itemUI	= itemObj.GetComponent<SongItemDisplay>();
				// var amount	= 1 - Mathf.Abs( itemObj.DistanceWithViewPortSnapCenter ) / 720f;
				// var scale	= Mathf.Clamp( amount, 0.4f, 1 );

				// itemUI.SetScale( scale );
			}
		}

		private LoopListViewItem2 OnUpdate( LoopListView2 view, int index )
		{
			if ( index < 0 || m_list.Count <= index ) return null;

			var data	= m_list[ index ];
			var itemObj	= view.NewListViewItem( m_original.name );
			var itemUI	= itemObj.GetComponent<SongItemDisplay>();
			// itemUI.songManager = 

			itemUI.Prime( data );
			// SongDisplay song = (SongDisplay)Instantiate(songDisplayPrefab);
        	// song.GetComponent<Canvas>().worldCamera = Camera.main;
        	// song.Prime(items);

			return itemObj;
		}
	}
}