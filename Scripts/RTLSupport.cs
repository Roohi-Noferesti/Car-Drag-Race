﻿using System.Text;
using TMPro;

// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace RTLTMPro
{
    public static class RTLSupport
    {
        public const int DefaultBufferSize = 2048;
        
        private static FastStringBuilder inputBuilder;
        private static FastStringBuilder glyphFixerOutput;
        
        static RTLSupport()
        {
            inputBuilder = new FastStringBuilder(DefaultBufferSize);
            glyphFixerOutput = new FastStringBuilder(DefaultBufferSize);
        }

        /// <summary>
        ///     Fixes the provided string
        /// </summary>
        /// <param name="input">Text to fix</param>
        /// <param name="output">Fixed text</param>
        /// <param name="fixTextTags"></param>
        /// <param name="preserveNumbers"></param>
        /// <param name="farsi"></param>
        /// <returns>Fixed text</returns>
        public static void FixRTL(
            string input,
            FastStringBuilder output,
            TMP_FontAsset fontAsset,
            bool farsi = true,
            bool fixTextTags = true, 
            bool preserveNumbers = false
        )
        {
            //we set this to true to temporary fix our tag and number problem . (wa wan english number in arabic language)
            preserveNumbers = true;
            if (fixTextTags)
                input = input.Replace("><", "> <");
            inputBuilder.SetValue(input);

            TashkeelFixer.RemoveTashkeel(inputBuilder);

            // The shape of the letters in shapeFixedLetters is fixed according to their position in word. But the flow of the text is not fixed.
            GlyphFixer.Fix(inputBuilder, glyphFixerOutput, preserveNumbers, farsi);

            //Restore tashkeel to their places.
            TashkeelFixer.RestoreTashkeel(glyphFixerOutput);

            // Fix flow of the text and put the result in FinalLetters field
            LigatureFixer.Fix(glyphFixerOutput, output,fontAsset, farsi, fixTextTags, preserveNumbers);
            
            if (fixTextTags)
            {
                RichTextFixer.Fix(output);
            
            }

            inputBuilder.Clear();
        }
 
    }
}