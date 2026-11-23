using System.Collections.Generic;
using Features.ItemsPack.Cards;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "CardsConfig", menuName = "Configs/CardsConfig", order = 66)]
    
    public class CardsConfigSo : ScriptableObject
    {
        public List<CardsSo> cards;
    }
}