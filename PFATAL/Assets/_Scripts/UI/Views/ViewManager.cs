using UnityEngine;

public class ViewManager : MonoBehaviour
{
    #region Singleton
    private static ViewManager instance;

    public static ViewManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ViewManager");
                instance = go.AddComponent<ViewManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }
    #endregion


    [SerializeField] View _startingView;

    View _currentView;

    private void Start()
    {
        SwapView(_startingView);
    }

    public void SwapView(View newView)
    {
        if(_currentView !=null)
            _currentView.Deactivate();

        _currentView = newView;

        _currentView.Activate();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
