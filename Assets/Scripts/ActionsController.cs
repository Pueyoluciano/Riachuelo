using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsController : MonoBehaviour
{
    [SerializeField] ActionItem takePicture;
    [SerializeField] ActionItem logbook;
    [SerializeField] ActionItem gallery;
    [SerializeField] ActionItem ponder;

    public ActionItem TakePicture { get => takePicture; }
    public ActionItem Logbook { get => logbook; }
    public ActionItem Gallery { get => gallery; }
    public ActionItem Ponder { get => ponder; }
}
