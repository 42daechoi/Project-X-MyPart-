public class RotateCommand : ICommand
{
    private Item item;

    public RotateCommand(Item item)
    {
        this.item = item;
    }

    public void Execute()
    {
        item.Rotate();
    }

    public void Undo()
    {
        for (int i = 0; i < 3; i++)
        {
            item.Rotate();
        }
    }
}