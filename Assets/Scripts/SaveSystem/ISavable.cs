namespace SaveSystem
{
    public interface ISavable<T>
    {
        T GetSavableData();

        void SetFromSavedData(T savedData);
    }
}