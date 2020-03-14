﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RouletteManager : MonoBehaviour
{

    MoveParameter moveParameter;

    Transform BattleUI = null;
    GameObject BattleUIObj;
    int result = 0;

    [SerializeField]
    GameObject BattleUIPrefab;
    [SerializeField]
    GameObject canvas;

    [SerializeField] private int rouletteId;
    Figure figure;
    BoardController boardController;
    LocalController localController;
    // Start is called before the first frame update
    void Start()
    {
        boardController = GameObject.Find("BoardMaster").GetComponent<BoardController>();
        localController = GameObject.Find("LocalMaster").GetComponent<LocalController>();
    }

    //ルーレットの生成
    public void MakeRoulette(Figure figure)
    {
        this.figure = figure;
        
        //初期化のため消去
        if(BattleUI != null)
        {
            Destroy(BattleUI.gameObject);
        }

        //ルーレットプレハブのインスタンス化
        // GameObject BattleUIObj = Instantiate(BattleUIPrefab);
        BattleUIObj = Instantiate(BattleUIPrefab);
        BattleUI = BattleUIObj.transform;
        BattleUI.SetParent(canvas.transform,false);


        Transform roulette = BattleUI.Find("Roulette").Find("Pieces");
        Transform partitions = BattleUI.Find("Roulette").Find("Partitions");
        int pieceNumber = 0;
        int sizeSum = 0;
        //各ピースの描画
        foreach (Transform piece in roulette)
        {
            if (pieceNumber < figure.pieces.Count)
            {
                GameObject pieceObj = piece.gameObject;
                Image pieceImg = pieceObj.GetComponent<Image>();

                //ピース幅の設定
                pieceImg.fillAmount = figure.pieces[pieceNumber].size / 96.0f;

                //色の設定
                //Debug.Log(figure.pieces[pieceNumber].color);
                switch (figure.pieces[pieceNumber].color)
                {
                    case MoveParameter.MoveOfColorName.White:
                        pieceImg.color = new Color(240 / 255f, 237 / 255f, 241 / 255f, 255 / 255f); break;
                    case MoveParameter.MoveOfColorName.Red:
                        pieceImg.color = new Color(230 / 255f, 80 / 255f, 80 / 255f, 255 / 255f); break;
                    case MoveParameter.MoveOfColorName.Blue:
                        pieceImg.color = new Color(87 / 255f, 198 / 255f, 244 / 255f, 255 / 255f); break;
                    case MoveParameter.MoveOfColorName.Purple:
                        pieceImg.color = new Color(201 / 255f, 112 / 255f, 230 / 255f, 255 / 255f); break;
                    case MoveParameter.MoveOfColorName.Gold:
                        pieceImg.color = new Color(230 / 255f, 205 / 255f, 86 / 255f, 255 / 255f); break;

                }

                //ピースを回転
                float rotateAmount = sizeSum * 360 / 96.0f;
                piece.Rotate(new Vector3(0, 0, -rotateAmount));

                //ピースのテキストをセット
                GameObject dmgTextObj = piece.Find("AttackText").Find("DamageText").gameObject;
                TextMeshProUGUI dmgText = dmgTextObj.GetComponent<TextMeshProUGUI>();
                if(figure.pieces[pieceNumber].color == MoveParameter.MoveOfColorName.White || figure.pieces[pieceNumber].color == MoveParameter.MoveOfColorName.Gold)
                {
                    dmgText.SetText(figure.pieces[pieceNumber].damage.ToString());
                }
                else if(figure.pieces[pieceNumber].color == MoveParameter.MoveOfColorName.Purple)
                {
                    string star = "";
                    for (int j = 0; j < figure.pieces[pieceNumber].numberOfStar; j++)
                    {
                        star += "☆";
                    }
                    dmgText.SetText(star);
                }
                
                //適切な位置までテキストを回転
                Transform atkTextObj = piece.Find("AttackText");
                atkTextObj.Rotate(new Vector3(0, 0, -figure.pieces[pieceNumber].size / 96.0f * 360.0f / 2.0f));

                //次のピースへ進む
                sizeSum += figure.pieces[pieceNumber].size;
                pieceNumber++;
            }

        }

        //変数初期化
        pieceNumber = 0;
        sizeSum = 0;

        //仕切りの描画
        foreach (Transform partition in partitions)
        {
            if (pieceNumber < figure.pieces.Count)
            {
                //仕切りを回転
                float rotateAmount = sizeSum * 360 / 96.0f;
                partition.Rotate(new Vector3(0, 0, -rotateAmount));

                //次の仕切りへ進む
                sizeSum += figure.pieces[pieceNumber].size;
                pieceNumber++;
            }

        }
    }

    //ルーレットを回転しさせて結果を表示
    public void SpinRoulette(int playerId)
    {

        //わざの抽選
        result = boardController.SpinResult[playerId];
        Attack resultAtk = this.figure.GetAttack(result);

        //ルーレットの初期化
        MakeRoulette(this.figure);
        GameObject roulette = BattleUI.Find("Roulette").gameObject;

        //回転
        iTween.RotateAdd(roulette, iTween.Hash("z", ((360.0f * 7.0f) + (result * 360.0f / 96.0f)),
                                               "time", 1.0f,
                                               "oncomplete","setAttackText",
                                               "oncompletetarget",this.gameObject,
                                               "easetype",iTween.EaseType.linear));//回転タイプや速度はここで指定


    }

    IEnumerator setAttackText()
    {
        Attack attack = this.figure.GetAttack(result);

        //ダメージテキストのセット
        GameObject dmgTextObj = BattleUI.Find("TextBase").Find("DamageText").gameObject;

        TextMeshProUGUI dmgText = dmgTextObj.GetComponent<TextMeshProUGUI>();
        if (boardController.GetMyPlayerId() != rouletteId)
        {
            dmgTextObj.transform.Rotate(0, 0, 180);
            dmgText.alignment = TextAlignmentOptions.Left;
        }
        if (attack.color == MoveParameter.MoveOfColorName.White || attack.color == MoveParameter.MoveOfColorName.Gold)
        {
            dmgText.SetText(attack.damage.ToString());
        }
        else if (attack.color == MoveParameter.MoveOfColorName.Purple)
        {
            string star = "";
            for (int j = 0; j < attack.numberOfStar; j++)
            {
                star += "☆";
            }
            dmgText.SetText(star);
        }

        //技名テキストのセット
        GameObject atkTextObj = BattleUI.Find("TextBase").Find("AttackNameText").gameObject;
        TextMeshProUGUI atkText = atkTextObj.GetComponent<TextMeshProUGUI>();
        if (boardController.GetMyPlayerId() != rouletteId)
        {
            atkTextObj.transform.Rotate(0, 0, 180);
            atkText.alignment = TextAlignmentOptions.Left;
        }

        atkText.SetText(attack.name);

        // 便宜上こうしてるけどクリックで遷移の方が望ましい
        yield return new WaitForSeconds(1);

        //ロック解除
        boardController.SetDoneFlagCustomProperty(boardController.GetMyPlayerId(), true);
    }
    public GameObject GetBatleUIObj()
    {
        return BattleUIObj;
    }

}