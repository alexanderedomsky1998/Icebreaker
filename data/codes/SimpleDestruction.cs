using Unigine;
using Console = Unigine.Console;

[Component(PropertyGuid = "c424b3d26173c109281a56d43f11d4234247ab26")]
public class SimpleDestruction : Component
{
    // Переменная в которой хранится ссылка на физику объекта
    private BodyFracture bodyFracture;

    // Переменная для указания применяемой физики разрушения
    [ShowInEditor] private int _typeDestruction = 0;

    /*
     * Переменная которая отражает количество
     * кусочков применяемая во внутренней логике
     */
    [ShowInEditor] private int _countPieces = 5;

    // Переменная для отражения включения физики после разрушения
    [ShowInEditor] private bool _disableGravity = false;

    // Переменная которая отражает текущее состояние разрушенности объекта
    private bool _destroyed = false;

    /*
     * Метод инициализации компонента
     *
     * В рамках данного метода происходит поиск физики объекта,
     * после чего ему присваивается метод обработки столкновения физики
     */
    private void Init()
    {
        // write here code to be called on component initialization
        bodyFracture = node.ObjectBody as BodyFracture;
        if (bodyFracture != null)
        {
            bodyFracture.AddContactEnterCallback(EnterCallback);
        }
    }

    /*
     * Базовый метод обновления компонента каждый кадр.
     *
     * В рамках данного кода он ничем не занят.
     */
    private void Update()
    {
        // write here code to be called before updating each render frame
    }

    /*
     * Метод обработки взаимодействия с физикой другого объекта.
     *
     * Данный метод вызывает в момент столкновения с физикой другого объекта,
     * затем если объект не разрушен выполняется внутреняя логика по разрушению.
     */
    void EnterCallback(Body body, int num)
    {
        if (_destroyed) return;

        /*
         * В зависимости от выбранного типа разрушения,
         * вызывается нужный метод разрушения
         */
        switch (_typeDestruction)
        {
            case 0:
                DestructAsShatter();
                break;
            case 1:
                DestructAsCrack(body, num);
                break;
            case 2:
                DestructAtSlice();
                break;
            default:
                DestructAsShatter();
                break;
        }

        // Отключение гравитации при разрушении, если необходимо
        if (_disableGravity)
        {
            bodyFracture.Gravity = false;
        }

        // Отключение взаимодействия с другими объектами коллизии
        bodyFracture.CollisionMask = 0;
        
        // Переключение состояния объекта разрушенности в true
        _destroyed = true;
        
        /*
         * Логика которая использовалась для поиска объекта
         * с которым происходит взаимодействия
         * и нанесение ему урона для замедления
         */
        var contactObject = body.GetContactObject(num);
        var rootObject = contactObject.RootNode;
        if (rootObject != null)
        {
            var baseController = rootObject.GetComponentInChildren<BaseController>();
            if (baseController != null)
            {
                // baseController.TakeDamage();
            }
        }
    }

    /*
     * Метод который вызывает физику движка для разрушения объекта
     * на определенное количество кусочков
     */
    void DestructAsShatter()
    {
        bodyFracture.CreateShatterPieces(_countPieces);
    }

    /*
     * Метод который вызывает физику движка для разрушения объекта на кружки
     *
     * Не предполагается к использованию.
     */
    private void DestructAsCrack(Body body, int num)
    {
        vec3 normal = Game.GetRandomFloat(-1f, 1f);
        var count = bodyFracture.CreateCrackPieces(body.Parent.Position, normal, 5, 2, 0.5f);
    }

    /*
     * Метод который подразумевал разрезание объекта.
     *
     * Не реализован.
     *
     * Не предполагается к использованию.
     */
    void DestructAtSlice()
    {
    }
}