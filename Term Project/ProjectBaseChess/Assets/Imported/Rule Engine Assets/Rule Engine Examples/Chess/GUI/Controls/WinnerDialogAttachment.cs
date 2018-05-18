using RuleEngine;
using RuleEngineAddons.TurnBased;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RuleEngineExamples.Chess {

    public class WinnerDialogAttachment : MonoBehaviour {

        public GameObject Dialog;
        public Text Message;
         
        void Awake() {

            // Self register with the gui controller and disable
            RuleEngineController.E.GetManager<GUIManager>().WinnerDialog = this;
            gameObject.SetActive(false);

        }

        public void Activate() {

            // Show and set on top of all other menu elements
            Dialog.SetActive(true);
            Dialog.transform.SetAsLastSibling();

            // Disable game interaction
            RuleEngineController.E.GetController<TurnController>().GameAcceptingUserInput = false;
            
        }

        public void SetText(string text) {
            Message.text = text;
        }
        
        public string GetStringForSinglePlayerWin(bool HasNextLevel) {
            string text = "";
            if (HasNextLevel)
                text = "Congrats, wanna keep going?";
            else
                text = "Congrats, you've finished the super amazing single player mode! Yeehaw!";
            return text;
        }

        public string GetStringForSinglePlayerLoss() {
            return "Uh oh!";
        }

        public string GetStringForMultiplayPlayer(List<Player> players, bool playersAreWinners) {
            string playerType = playersAreWinners ? "winner" : "loser";

            string text = "";
            if (players.Count == 1)
                text = "The " + playerType + " is " + players[0].Name + "!";
            if (players.Count == 2)
                text = "The " + playerType + "s are " + players[0].Name + " and " + players[0].Name + "!";
            else if (players.Count > 2) {
                text = "The " + playerType + "s are ";
                for (int i = 0; i < players.Count - 1; i++) {
                    if (i != 0)
                        text += ", ";
                    text += players[i];
                }
                text += " and " + players[players.Count - 1] + "!";
            }
            return text;
        }


    }

}