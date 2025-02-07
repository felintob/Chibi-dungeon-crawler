using UnityEngine;
using UnityEngine.SceneManagement;
using Music;

namespace UI {
  public class CreditScreenUI : MonoBehaviour {
    
    public FadeMusic fadeMusic;




    public void FadeOutMusic(){
      fadeMusic.FadeOut();
    }

    public void SwitchToTitleScreen(){
      SceneManager.LoadScene("Scenes/TitleScreen");
    }
  }
}