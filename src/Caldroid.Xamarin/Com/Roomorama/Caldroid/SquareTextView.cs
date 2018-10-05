using System;
using Android.Content;
using Android.Runtime;
using Android.Util;

namespace Caldroid.Xamarin.Com.Roomorama.Caldroid
{
    public class SquareTextView : CellView
    {
        public SquareTextView(Context context) 
            : base(context)
        {

        }


        public SquareTextView(Context context, IAttributeSet attrs) 
            : base(context, attrs)
        {

        }


        public SquareTextView(Context context, IAttributeSet attrs, int defStyleAttr) 
            : base(context, attrs, defStyleAttr)
        {

        }


        protected SquareTextView(IntPtr javaReference, JniHandleOwnership transfer) 
            : base(javaReference, transfer)
        {

        }


        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }


        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
        }
    }
}