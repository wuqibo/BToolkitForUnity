

//该方法要先写，否则下方函数调用不到
extern "C" char* MakeStringCopy_Copy(const char* string){
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

extern "C" void _Copy(const char *text){
    NSString *textStr = [[NSString alloc] initWithUTF8String:text];
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    pasteboard.string = textStr;
}

extern "C" char* _GetPaste(){
	UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    NSString *pasteText = pasteboard.string;
    //返回字符串需要分配内存，否则原有数据内存回收后会报错
    return MakeStringCopy_Copy([pasteText UTF8String]);
}
