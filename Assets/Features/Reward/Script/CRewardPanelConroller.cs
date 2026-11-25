using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CRewardPanelConroller : MonoBehaviour
{
    [System.Serializable]
    private struct SReawrdSelectButton
    {
        public TMP_Text _nameText;
        public TMP_Text _targetText;
        public TMP_Text _subTargetText;
        public TMP_Text _discriptionText;
        public Image _rewardImage;
        public Button _rewardButton;
    }

    #region inspector
    [SerializeField] private Button _rewardButtonPrefab;
    [SerializeField] private Transform _rewardButtonLayout;
    [SerializeField] private SORewardPoolData _rewardPoolData;

    [SerializeField] private SReawrdSelectButton[] _rewardSelectButton;
    #endregion

    #region private var
    private IRewardable[] _rewards;
    #endregion

    public void CreatRewardButton(int amound)
    {
        for (int i = 0; i < amound; i++)
        {
            Button rewardButton = Instantiate<Button>(_rewardButtonPrefab, _rewardButtonLayout);

            rewardButton.onClick.AddListener(OpenRewardPanal);
            rewardButton.onClick.AddListener(() => { Destroy(rewardButton); });
            rewardButton.onClick.AddListener(rewardButton.onClick.RemoveAllListeners);
        }
    }

    public void OpenRewardPanal()
    {
        gameObject.SetActive(true);

        CWeightedRandom<IRewardable> rewaedPool = _rewardPoolData.GetRewardPool();

        _rewards = new IRewardable[3];

        for (int i = 0; i < _rewards.Length; i++)
        {
            _rewards[i] = rewaedPool.GetRandomItemThenRemoveFormPool();

            if (_rewards[i] == null)
            {
                Debug.Log("획득 가능한 보상이 부족합니다.");
                _rewardSelectButton[i]._rewardButton.gameObject.SetActive(false);
            }

            _rewardSelectButton[i]._nameText.text = _rewards[i].Name;
            _rewardSelectButton[i]._targetText.text = _rewards[i].TargetName;
            _rewardSelectButton[i]._subTargetText.text = _rewards[i].SubName;
            _rewardSelectButton[i]._discriptionText.text = _rewards[i].Description;
            _rewardSelectButton[i]._rewardImage.sprite = _rewards[i].RewardSprite;
        }

    }

    public void GetReward(int index)
    {
        gameObject.SetActive(false);

        if (_rewards.Length <= index || _rewards[index] == null)
        {
            Debug.LogWarning("out of index range or _rewards is null.");
            return;
        }

        if (_rewards[index] is SOUpgradeData upgradeData)
        {
            CGameProgressManager.Instance.UpgradeList.Add(upgradeData);
        }
    }
}
