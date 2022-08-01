#ifndef MYHLSINCLUDE_INCLUDED
#define MYHLSINCLUDE_INCLUDED

void CalcDmgMult_float(float2 uv, float falloffMult, out float alpha){
    float lengthVal = 1.0f;
        
    // calc far dmg falloff
    if (uv.y >= 0.75f){
        lengthVal *= (1.0f - ((uv.y - 0.75f)/0.25f));
    }

    // calc close dmg falloff
    if (uv.y <= 0.1){
        lengthVal *= (1.0f - ((0.1f - uv.y)/0.1f));
    }

    // if behind or too far, no dmg
    if (uv.y > 1.0 || uv.y < 0.0){
        alpha = 0.0f;
        return;
    }
    
    // make width var be 1 in the middle, and reach 0 at sides
    float widthVal = 1.0f;
    float currentWidth = uv.x + 0.5f;
    if (currentWidth > 1.0f){
        currentWidth = 1.0f - (currentWidth - 1.0f);
    }

    // calc width dmg falloff
    if (currentWidth > 0.5){
        widthVal = currentWidth * 2.0f - 1.0f;
    }

    // if too wide, no dmg
    if (currentWidth <= 0.5f){
        alpha = 0.0f;
        return;
    }

    // use falloff to calculate horiz dmg
    widthVal = lerp(1.0 - falloffMult, 1.0, widthVal);
    widthVal = clamp(widthVal, 0.0f, 1.0f);

    // return
    alpha = (lengthVal * widthVal);
}

#endif // MYHLSINCLUDE_INCLUDED