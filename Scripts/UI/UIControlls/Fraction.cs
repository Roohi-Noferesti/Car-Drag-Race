public struct Fraction
{
    public int Numerator;

    public int Denominator;

    public float ToPercent()
    {
        return (this.Denominator != 0) ? ((float)this.Numerator / (float)this.Denominator) : 0f;
    }
}