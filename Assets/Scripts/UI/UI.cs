using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour
{
	public GameObject anchor;
	public GameObject goodsDialog;

	public void OnGoodsButtonClick()
	{
		NGUITools.SetActive( transform.gameObject, false );
		NGUITools.SetActive( goodsDialog, true );
	}

	public void OnDismissButtonClick()
	{
		NGUITools.SetActive( goodsDialog, false );
		NGUITools.SetActive( transform.gameObject, true );
	}
}