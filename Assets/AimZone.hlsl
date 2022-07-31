#ifndef MYHLSINCLUDE_INCLUDED
#define MYHLSINCLUDE_INCLUDED

void CalcDmgMult_float(float2 uv, out float alpha){
    float lengthVal = 1.0;
        
    if (uv.y >= 0.75){
        lengthVal *= (1.0 - ((uv.y - 0.75)/0.25));
    }

    if (uv.y <= 0.1){
        lengthVal *= (1.0 - ((0.1 - uv.y)/0.1));
    }

    if (uv.y > 1.0 || uv.y < 0.0){
        lengthVal = 0.0;
    }
    
    float widthVal = 1.0;
    float currentWidth = uv.x + 0.5;
    if (currentWidth > 1.0){
        currentWidth = 1.0 - (currentWidth - 1.0);
    }
    if (currentWidth > 0.5){
        widthVal = currentWidth * 2.0 - 1.0;
    }
    
    alpha = (lengthVal * widthVal);
}

#endif // MYHLSINCLUDE_INCLUDED