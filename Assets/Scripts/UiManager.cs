using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artificial_Intelligence;
public class UiManager : MonoBehaviour
{
    public CameraController.Transition playerCharTransition;
    public PlayerCharacter PlayerChar;
    public GameObject TapPlayButton, ReplayButton,DoneButton,PaintWall;
    public Ranker ranker;
    public MessageText messager;
    public static UiManager manager;

    public void Awake()
    {
        manager = this;

    }

    public void TapToPlay()
    {
        messager.ShowMessage("Hurry!", 3f);
        TapPlayButton.SetActive(false);
        PlayerChar.SetMovenet(true);
    }
    public void LevelFail()
    {
        messager.ShowMessage("Better Luck next time!", 3f);
        ReplayButton.SetActive(true);
    }
    public void Replay()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void Finish()
    {
       // DoneButton.SetActive(true);
        messager.ShowMessage("Congratulations!",2f);
        messager.ShowMessage("Draw Your Signature",2f);
        ranker.StopRanking();
    }
    public void FinishPainting()
    {
        playerCharTransition.MakeTransition();
        DoneButton.SetActive(false);
        CameraController.camera.GetComponent<Es.InkPainter.Sample.MousePainter>().enabled = false;
        ReplayButton.SetActive(true);

        Texture texture = PaintWall.GetComponent<MeshRenderer>().materials[0].GetTexture("_BaseMap");
        RenderTexture.active = texture as RenderTexture;
        Texture2D tx = new Texture2D(texture.width, texture.height);
        tx.ReadPixels(new Rect(0, 0, tx.width, tx.height), 0, 0);
        tx.Apply();
        //Texture2D tx = texture as Texture2D;  // Texture2D.CreateExternalTexture(texture.width, texture.height, TextureFormat.RGBA32, true, false, texture.GetNativeTexturePtr());
        
        int redCount = 0;
        for (int x = 0; x < tx.width; x++)
        {
            for (int y = 0; y < tx.height; y++)
            {
                if (tx.GetPixel(x, y) == Color.red)
                    redCount++;
            }
        }
        float percent = redCount * 100 / (texture.width * texture.height);
        messager.ShowMessage("You've drawn " + percent + "% of the canvas",10f);
        Debug.Log("You've drawn " + percent + "% of the canvas");
    }
}
