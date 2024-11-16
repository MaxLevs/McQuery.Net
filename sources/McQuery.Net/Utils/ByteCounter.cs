namespace McQuery.Net.Utils;

internal class ByteCounter
{
    private readonly byte[] countUnits;

    public ByteCounter()
    {
        countUnits = new byte[4];
        Reset();
    }

    public bool GetNext(byte[] receiver)
    {
        for (int i = 0; i < countUnits.Length; ++i)
        {
            if (countUnits[i] < 0x0F)
            {
                countUnits[i]++;
                countUnits.CopyTo(receiver, 0);

                return true;
            }

            countUnits[i] = 0x00;
        }

        return false;
    }

    public void Reset()
    {
        for (int i = 0; i < countUnits.Length; ++i) countUnits[i] = 0;
    }
}
