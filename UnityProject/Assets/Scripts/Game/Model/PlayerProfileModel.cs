using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Extensions;

namespace Game.Models
{
    public class PlayerProfileModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public ObscuredInt avatarIndex;
            public ObscuredString name;
            public ObscuredInt score;
            public ObscuredBool gender;

            public void SetAvatarIndex(int AvatarIndex)
            {
                avatarIndex = AvatarIndex;
                ChangeData();
            }

            public void SetName(string Name)
            {
                name = Name;
                ChangeData();
            }

            public void SetSCore(int Score)
            {
                score = Score;
                ChangeData();
            }

            public void SetGender(bool Gender)
            {
                gender = Gender;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

        [SerializeField] private Sprite[] _maleAvatarSprites;
        [SerializeField] private Sprite[] _femaleAvatarSprites;
        [SerializeField] private string[] _nicknames;

#pragma warning restore 0649
        #endregion

        public StorageModel StorageModel => _storageModel;
        public Sprite[] MaleSprites => _maleAvatarSprites;
        public Sprite[] FemaleSprites => _femaleAvatarSprites;
        public Sprite PlayerAvatar
        {   
            get
            {
                if(PlayerGender)
                {
                    if(!MaleSprites.IndexOutOfRange(Index))
                        return MaleSprites[Index];
                    else
                        return MaleSprites[0];
                }
                else
                {
                     if(!FemaleSprites.IndexOutOfRange(Index))
                        return FemaleSprites[Index];
                    else
                        return FemaleSprites[0];
                }
            }
        }
        public string[] Nicknames => _nicknames;

        public bool HasDeadInSession { set; get; } = false;

        public int Index
        {
            get => _data.avatarIndex;
            protected set => _data.SetAvatarIndex(value);
        }

        public string PlayerName
        {
            get => _data.name;
            protected set => _data.SetName(value);
        }

        public int PlayerScore
        {
            get => _data.score;
            protected set => _data.SetSCore(value);
        }

        public bool PlayerGender
        {
            get => _data.gender;
            protected set => _data.SetGender(value);
        }

        public void GenerateNextAvatarIndex()
        {
            Index = PlayerGender ?
                Random.Range(0, _maleAvatarSprites.Length) :
                Random.Range(0, _femaleAvatarSprites.Length);
        }

        public void GenerateNextName()
        {
            SetPlayerName(Nicknames[Random.Range(0, _nicknames.Length)]);
        }

        public void ResetScore() => PlayerScore = 0;

        private void OnEnable() => StorageModel.TryProcessing(_data);

        public void SetPlayerScore(int score) => PlayerScore = score;

        public void SetPlayerName(string name) => PlayerName = name;

        public void SetPlayerGender(bool gender) => PlayerGender = gender;
    }
}
