using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class BoardUIButton : MonoBehaviour
{
    public int ID { get; private set; }

    [SerializeField] public Button _button;

    public void Init(int id, System.Action<int> clickEvent)
    {
        ID = id;
        if (_button == null)
            _button = GetComponent<Button>();
        _button.onClick.AddListener(delegate { clickEvent.Invoke(ID); });
    }
}
