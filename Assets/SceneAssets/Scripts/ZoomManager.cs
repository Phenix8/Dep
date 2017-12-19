using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ZoomManager : MonoBehaviour
{
    #region Attributs

    public List<GameObject> triggersList;
    public GameObject globalCanvas;
    public GameObject emitersList;
    public GameObject canvasToMove;

    public Transform cursorTransform;

    public float borderMoveTolerance = 50.0f;
    public float moveOffset = 5.0f;
    public float dezoomRatio = 0.5f;
    public float zoomSpeed = 1.0f;
    private float startTimeZoom;

    private Vector3 startCanvasToMoveScale;
    private Vector3 zoomScaleToApply;

    private Vector2 zoomPositionToApply;

    private int currentActiveTriggersNb = 0;

    private RectTransform canvasRectToMove;

    public bool isInZoom = false;

    #endregion

    #region Méthodes

    public void Start()
    {
        canvasRectToMove = canvasToMove.GetComponent<RectTransform>();
        startCanvasToMoveScale = canvasRectToMove.localScale;
    }


    public void Update()
    {
        CheckMovement();
        UpdateZoomLevel();

        if (isInZoom && Time.time - startTimeZoom > zoomSpeed)
            isInZoom = false;
        
        print("Ratio camera : 1/" + (int)(canvasRectToMove.localScale.x / 0.078));
    }


    // Vérifie si le pointeur lumineux est sur une des bordures
    // et que l'on n'a pas atteint les bords du tableau
    private void CheckMovement()
    {
        RectTransform renderRect = globalCanvas.GetComponent<RectTransform>();
        float canvasWidthLimit = canvasRectToMove.rect.width / 2 + renderRect.rect.width / 2;
        float canvasHeightLimit = canvasRectToMove.rect.height / 2 + renderRect.rect.height / 2;
        Vector3 cursorPosition = new Vector3(cursorTransform.position.x, cursorTransform.position.y, 0.0f); 

        // gauche
        if (cursorPosition.x < renderRect.rect.xMin + borderMoveTolerance   &&  canvasRectToMove.anchoredPosition.x < canvasWidthLimit)
        
            canvasRectToMove.anchoredPosition = emitersList.transform.localPosition = new Vector2(canvasRectToMove.anchoredPosition.x + moveOffset, canvasRectToMove.anchoredPosition.y);
        
        // droite
        else if (cursorPosition.x > renderRect.rect.xMax - borderMoveTolerance  &&  canvasRectToMove.anchoredPosition.x > -canvasWidthLimit)
        
            canvasRectToMove.anchoredPosition = emitersList.transform.localPosition = new Vector2(canvasRectToMove.anchoredPosition.x - moveOffset, canvasRectToMove.anchoredPosition.y);
        
        // bas
        else if (cursorPosition.y < renderRect.rect.yMin + borderMoveTolerance  &&  canvasRectToMove.anchoredPosition.y < canvasHeightLimit)

            canvasRectToMove.anchoredPosition = emitersList.transform.localPosition = new Vector2(canvasRectToMove.anchoredPosition.x, canvasRectToMove.anchoredPosition.y + moveOffset);
        
        // haut
        else if (cursorPosition.y > renderRect.rect.yMax - borderMoveTolerance  &&  canvasRectToMove.anchoredPosition.y > -canvasHeightLimit)

            canvasRectToMove.anchoredPosition = emitersList.transform.localPosition = new Vector2(canvasRectToMove.anchoredPosition.x, canvasRectToMove.anchoredPosition.y - moveOffset);       
    }


    // Compte les triggers activés et met à jour le niveau de
    // zoom si besoin
    private void UpdateZoomLevel()
    {
        int nextActiveTriggersNb = triggersList.Where(x => x.GetComponent<TriggerScript>().isActive).Count();
        int dezoomTriggers = (int)dezoomRatio * nextActiveTriggersNb;
        Vector3 newScale, newPosition;

        // On ne met à jour que si le nombre de triggers a changé
        if (currentActiveTriggersNb == nextActiveTriggersNb || isInZoom)
            return;

        Debug.LogError("Updating zoom from " + currentActiveTriggersNb + " triggers to " + nextActiveTriggersNb + " triggers");

        // zoom à l'état initial
        if (nextActiveTriggersNb == 0)
        {
            newScale = startCanvasToMoveScale;
            newPosition = new Vector3(canvasRectToMove.anchoredPosition.x * (int)dezoomRatio, canvasRectToMove.anchoredPosition.y * (int)dezoomRatio, 1.0f);
        }

        // zoom
        else if (nextActiveTriggersNb < currentActiveTriggersNb)
        {
            newScale = new Vector3(canvasRectToMove.localScale.x * (int)dezoomRatio, canvasRectToMove.localScale.y * (int)dezoomRatio, 1.0f);
            newPosition = new Vector3(canvasRectToMove.anchoredPosition.x * (int)dezoomRatio, canvasRectToMove.anchoredPosition.y * (int)dezoomRatio, 1.0f);
            print("From " + canvasToMove.GetComponent<RectTransform>().position + " to " + newPosition);
        }

        // dézoom
        else
        {
            newScale = new Vector3(startCanvasToMoveScale.x / dezoomTriggers, startCanvasToMoveScale.y / dezoomTriggers, 1.0f);
            newPosition = new Vector3(canvasRectToMove.anchoredPosition.x / (int)dezoomRatio, canvasRectToMove.anchoredPosition.y / (int)dezoomRatio, 1.0f);
            CheckPictureLimits();
        }

        startTimeZoom = Time.time;
        isInZoom = true;
        iTween.ScaleTo(canvasToMove, newScale, zoomSpeed);
        iTween.MoveTo(canvasToMove, newPosition, zoomSpeed);

        // Mise à jour du niveau de dézoom actuel
        currentActiveTriggersNb = nextActiveTriggersNb;
        canvasRectToMove = canvasToMove.GetComponent<RectTransform>();
    }


    // Vérifie les limites du tableau après dézoom
    private void CheckPictureLimits()
    {
        RectTransform renderRect = globalCanvas.GetComponent<RectTransform>();
        float canvasWidthLimit = canvasRectToMove.rect.width / 2 + renderRect.rect.width / 2 - 10.0f;
        float canvasHeightLimit = canvasRectToMove.rect.height / 2 + renderRect.rect.height / 2 - 10.0f;

        // limite gauche
        if (canvasRectToMove.anchoredPosition.x > canvasWidthLimit)
        {
            Debug.LogError("Forcing position after dezoom");
            canvasRectToMove.anchoredPosition = emitersList.transform.localPosition = new Vector2(canvasWidthLimit - 10.0f, canvasRectToMove.anchoredPosition.y);
        }

        // limite droite
        else if (canvasRectToMove.anchoredPosition.x < -canvasWidthLimit)
        {
            Debug.LogError("Forcing position after dezoom");
            canvasRectToMove.anchoredPosition = emitersList.transform.localPosition = new Vector2(canvasWidthLimit + 10.0f, canvasRectToMove.anchoredPosition.y);
        }

        // limite basse
        else if (canvasRectToMove.anchoredPosition.y > canvasHeightLimit)
        {
            Debug.LogError("Forcing position after dezoom");
            canvasRectToMove.anchoredPosition = emitersList.transform.localPosition = new Vector2(canvasRectToMove.anchoredPosition.x, canvasHeightLimit - 10.0f);
        }

        // limite haute
        else if (canvasRectToMove.anchoredPosition.y < -canvasHeightLimit)
        {
            Debug.LogError("Forcing position after dezoom");
            canvasRectToMove.anchoredPosition = emitersList.transform.localPosition = new Vector2(canvasRectToMove.anchoredPosition.x, canvasHeightLimit + 10.0f);
        }

    }

    #endregion
}

