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
        // Controlliamo il puzzle solo se la porta è ancora chiusa
        if (!portaAperta)
        {
            ControllaPuzzle();
        }
    }

    void ControllaPuzzle()
    {
        // 1. Controlla che le mattonelle giuste siano TUTTE accese
        foreach (FloorToggle mattonella in mattonelleGiuste)
        {
            if (!mattonella.isOn) 
            {
                return; // Ne manca almeno una, blocca il controllo
            }
        }

        // 2. Controlla che le mattonelle sbagliate siano TUTTE spente
        foreach (FloorToggle mattonella in mattonelleSbagliate)
        {
            if (mattonella.isOn) 
            {
                return; // È stata calpestata una mattonella di troppo, blocca il controllo
            }
        }

        // 3. Se arriva qui, la combinazione è ESATTA
        ApriPorta();
    }

    void ApriPorta()
    {
        portaAperta = true;
        Debug.Log("Combinazione esatta! La porta si apre.");
        
        // Disattiva il GameObject della porta per far passare il giocatore
        gameObject.SetActive(false); 
    }
}