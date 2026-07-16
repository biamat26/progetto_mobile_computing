using UnityEngine;

public class GestorePortaPuzzle : MonoBehaviour
{
    [Header("Mattonelle che DEVONO essere accese (il disegno azzurro)")]
    public FloorToggle[] mattonelleGiuste;

    [Header("Mattonelle che DEVONO restare spente (lo sfondo)")]
    public FloorToggle[] mattonelleSbagliate;

    private bool portaAperta = false;

    void Update()
    {
        if (!portaAperta)
        {
            ControllaPuzzle();
        }
    }

    void ControllaPuzzle()
    {
        foreach (FloorToggle mattonella in mattonelleGiuste)
        {
            if (!mattonella.isOn)
                return;
        }

        foreach (FloorToggle mattonella in mattonelleSbagliate)
        {
            if (mattonella.isOn)
                return;
        }

        ApriPorta();
    }

    void ApriPorta()
    {
        portaAperta = true;

        // blocca tutte le mattonelle così restano fisse nella combinazione corretta
        foreach (FloorToggle mattonella in mattonelleGiuste)
            mattonella.BloccaMattonella();

        foreach (FloorToggle mattonella in mattonelleSbagliate)
            mattonella.BloccaMattonella();

        TerminalPanel tp = GetComponentInParent<TerminalPanel>();
        if (tp == null) tp = FindObjectOfType<TerminalPanel>();
        if (tp != null) tp.NotifyDoorOpen();

        gameObject.SetActive(false);
    }
}