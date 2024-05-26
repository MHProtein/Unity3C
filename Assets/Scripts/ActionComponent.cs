
public abstract class ActionComponent
{
    public bool tick;
    protected Movement _movement;
    public int order = 0;

    public delegate void OnActionPerform();
    public event OnActionPerform onActionPerformed;
    
    public delegate void OnActionCancel();
    public event OnActionCancel onActionCanceled;
    
    public virtual void Perform()
    {
        onActionPerformed?.Invoke();
    }
    
    public virtual void Start()
    {
    }
    
    public virtual void Cancel()
    {
        onActionCanceled?.Invoke();
    }

    public virtual void SetMovement(Movement movement)
    {
        _movement = movement;
    }
    
    public virtual void FixedUpdate()
    {
    }
}
