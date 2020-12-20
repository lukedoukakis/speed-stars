using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardEntryButtonController : MonoBehaviour
{
	
	public LeaderboardManager lbm;
	public Button button;
	public Image image;
	
	public int raceEvent;
	public string pfid;
	public int position;
	public string userName;
	public float score;
	public string racerName;
	public string date;
	
	
	
	public void init(LeaderboardManager _lbm, int _raceEvent, string _pfid, int _position, string _userName, float _score, string _racerName, string _date){
		this.lbm = _lbm;
		this.button = GetComponent<Button>();
		this.image = GetComponent<Image>();
		
		this.raceEvent = _raceEvent;
		this.pfid = _pfid;
		this.position = _position;
		this.userName = _userName;
		this.score = _score;
		this.racerName = _racerName;
		this.date = _date;
		transform.Find("Text_Placing").gameObject.GetComponent<TextMeshProUGUI>().text = (position+1).ToString();
		transform.Find("Text_Username").gameObject.GetComponent<TextMeshProUGUI>().text = userName;
		transform.Find("Text_Racername").gameObject.GetComponent<TextMeshProUGUI>().text = racerName;
		
		string format;
		string units;
		if(_raceEvent <= 3){ format = "F3"; units = " sec"; }
		else{ format = "F0"; units = " pts"; }
		transform.Find("Text_Score").gameObject.GetComponent<TextMeshProUGUI>().text = score.ToString(format) + units;
		
		if(_raceEvent > 3){
			button.interactable = false;
		}
	}
	
	public void downloadRacer(){
		StartCoroutine(lbm.downloadRacer(this.raceEvent, this.pfid));
		image.color = new Color(.37f, .37f, 1f); 
	}
	
	public void showTooltip(){
		TooltipController ttc = lbm.tooltipController;
		string s = "Set by " + userName + " on " + date + "\nAthlete: <color=#b8daff>" + racerName + "</color>";
		
		if(raceEvent <= 3){
			s += "\n\n<color=yellow>Click to download ghost</color>";
		}
		else{
			s += "\n\n<color=yellow>(Not downloadable)</color>";
		}
		ttc.show(-1f);
		ttc.setText(s);
	}
	
	public void hideTooltip(){
		lbm.tooltipController.hide();
	}
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
