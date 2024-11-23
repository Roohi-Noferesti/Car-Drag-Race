using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserImageData 
{
    public long UserID { get; private set; }

    public string ImageUrl { get; private set; }

    public Texture ImageCache { get; private set; }

    public UserImageSize ImageSize { get; private set; }


    public UserImageData() { }

    public UserImageData(long userID,string imageUrl,Texture image,UserImageSize imageSize)
    {
        UserID = userID;
        ImageUrl = imageUrl;
        ImageCache = image;
        ImageSize = imageSize;
    }

}


public enum UserImageSize
{
    Size_50 = 50,
    Size_100 = 100,
    Size_200 = 200,
    Size_500 = 500,
}
