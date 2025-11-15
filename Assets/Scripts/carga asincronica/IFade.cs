using System.Collections;
public interface IFade
{
    IEnumerator FadeOut(float duration);
    IEnumerator FadeIn(float duration);
}
