using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Uplauncher.Enums;

namespace Uplauncher.Managers
{
    public class ImageManager
    {
        private readonly ImageSource[] _states;
        private readonly ImageSource[] _backgrounds;
        private readonly BitmapImage _paginationHover;
        private readonly BitmapImage _pagination;
        private short _currentImageId;

        private readonly Image _play;

        private readonly Image _pgBackground;

        private readonly Image _background;

        private readonly Image _state;

        private readonly Image[] _slides;

        public ImageManager(Image pgBackground, Image play, Image state, Image background, Image[] slides)
        {
            this._pgBackground = pgBackground;
            this._play = play;
            this._state = state;
            this._background = background;
            this._slides = slides;
            this._currentImageId = 3;

            this._paginationHover = new BitmapImage(new Uri(@"/assets/theme/slider/pagination_hover.png", UriKind.Relative));
            this._pagination = new BitmapImage(new Uri(@"/assets/theme/slider/pagination.png", UriKind.Relative));

            this._states = new ImageSource[]
            {
                new BitmapImage(new Uri(@"/assets/theme/dofus/dofus_vert.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/assets/theme/dofus/dofus_jaune.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/assets/theme/dofus/dofus_rouge.png", UriKind.Relative))
            };

            this._backgrounds = new ImageSource[]
            {
                new BitmapImage(new Uri(@"/assets/theme/background/1.jpg", UriKind.Relative)),
                new BitmapImage(new Uri(@"/assets/theme/background/2.jpg", UriKind.Relative)),
                new BitmapImage(new Uri(@"/assets/theme/background/3.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/assets/theme/background/4.jpg", UriKind.Relative)),
                new BitmapImage(new Uri(@"/assets/theme/background/5.jpg", UriKind.Relative))
            };
        }

        public void UpdatePagination(MouvementTransition mouvement)
        {
            short currentSlide = default;
            short previousSlide = default;

            if (mouvement == MouvementTransition.Next)
            {
                currentSlide = (short)(_currentImageId + 1);
                previousSlide = _currentImageId;
                if (currentSlide == 6) currentSlide = 1;
            }
            else
            {
                currentSlide = (short)(_currentImageId - 1);
                previousSlide = _currentImageId;

                if (currentSlide == 0) currentSlide = 5;
            }

            _slides[currentSlide - 1].Source = _paginationHover;
            _slides[previousSlide - 1].Source = _pagination;
            _background.Source = _backgrounds[currentSlide - 1];
            _currentImageId = currentSlide;
        }

        public void UpdateUplauncherState(UplauncherState state)
        {
            switch (state)
            {
                case UplauncherState.Update:
                    _state.Source = _states[0];
                    break;
                case UplauncherState.Updating:
                    _state.Source = _states[1];
                    break;
                case UplauncherState.Dirty:
                    _state.Source = _states[2];
                    break;
            }
        }

        public void UpdatePlayButton(bool enabled)
        {
            if (enabled)
                _play.Visibility = Visibility.Visible;
            else
                _play.Visibility = Visibility.Hidden;

        }

        public void UpdateProgressBar(bool enabled)
        {
            if (enabled)
                _pgBackground.Visibility = Visibility.Visible;
            else _pgBackground.Visibility = Visibility.Hidden;
        }
    }
}
