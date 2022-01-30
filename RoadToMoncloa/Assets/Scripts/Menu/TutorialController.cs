using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject[] _slides;
    [SerializeField] private Button _nextSlideButton;
    [SerializeField] private Button _prevSlideButton;

    private int _currentSlideIndex;

    public void OpenTutorial()
    {
        _nextSlideButton.gameObject.SetActive(true);
        OpenSlide(0);
    }

    public void OpenNextSlide()
    {
        _slides[_currentSlideIndex].SetActive(false);

        if (_currentSlideIndex >= _slides.Length - 1)
        {
            FinishTutorial();
        }
        else
        {
            OpenSlide(_currentSlideIndex + 1);
        }
    }

    public void OpenPreviousSlide()
    {
        _slides[_currentSlideIndex].SetActive(false);

        OpenSlide(_currentSlideIndex - 1);
    }

    private void OpenSlide(int slideIndex)
    {
        _currentSlideIndex = slideIndex;
        _slides[_currentSlideIndex].SetActive(true);

        _prevSlideButton.gameObject.SetActive(_currentSlideIndex > 1);
    }

    private void FinishTutorial()
    {
        _nextSlideButton.gameObject.SetActive(false);
        _prevSlideButton.gameObject.SetActive(false);
    }
}