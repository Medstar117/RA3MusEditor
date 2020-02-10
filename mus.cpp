#include <stdio.h>

inline void Swap(unsigned short& Value)
{
	Value = (Value & 0xFF00) >> 8 | (Value & 0x00FF) << 8;
	return;
}

int main(int argc, char* argv[])
{
	FILE *f = fopen(argv[1],"rb");
	fseek(f,0,SEEK_END);
	int sz = ftell(f);
	fseek(f,0,SEEK_SET);
	char *buffer = new char[sz];
	fread(buffer,sz,1,f);
	fclose(f);
	char *buf = buffer;
	unsigned int i;
	unsigned int count;
	unsigned char b;
	unsigned short s;
	i = *(unsigned int *)buf;
	printf("Unknown1 %x\n",i);
	buf += 4;
	count = *(unsigned int *)buf;
	printf("count %d\n",count);
	buf += 4;
	i = *(unsigned int *)buf;
	printf("Unknown2 %x\n",i);
	buf += 4;
	i = *(unsigned int *)buf;
	buf += 4;
	printf("Name %S\n",buf);
	buf += i;
	buf += 2;
	for (unsigned int z = 0;z < count;z++)
	{
		i = *(unsigned int *)buf;
		printf("Entry %d Unknown1 %x\n",z,i);
		buf += 4;
		i = *(unsigned int *)buf;
		printf("Entry %d Index %x\n",z,i);
		buf += 4;
		i = *(unsigned int *)buf;
		printf("Entry %d Unknown2 %x\n",z,i);
		buf += 4;
		i = *(unsigned int *)buf;
		printf("Entry %d Unknown3 %x\n",z,i);
		buf += 4;
		i = *(unsigned int *)buf;
		printf("Entry %d Unknown4 %x\n",z,i);
		buf += 4;
		i = *(unsigned int *)buf;
		printf("Entry %d Unknown5 %x\n",z,i);
		buf += 4;
		i = *(unsigned int *)buf;
		printf("Entry %d Unknown6 %x\n",z,i);
		buf += 4;
	}
	while ((*(unsigned int *)buf) == 0)
	{
		buf += 0x1C;
	}
	for (unsigned int z = 0;z < count;z++)
	{
		b = *(unsigned char *)buf;
		printf("Header %d Codec %x\n",z,b);
		buf += 1;
		b = *(unsigned char *)buf;
		printf("Header %d Channels %x\n",z,b);
		buf += 1;
		s = *(unsigned short *)buf;
		Swap(s);
		printf("Header %d SampleRate %x\n",z,s);
		buf += 2;
		s = *(unsigned short *)buf;
		Swap(s);
		printf("Header %d Unknown %x\n",z,s);
		buf += 2;
		s = *(unsigned short *)buf;
		Swap(s);
		printf("Header %d Samples %x\n",z,s);
		buf += 2;
		i = *(unsigned int *)buf;
		printf("Header %d Unknown2 %x\n",z,i);
		buf += 4;
		i = *(unsigned int *)buf;
		printf("Header %d Unknown3 %x\n",z,i);
		buf += 4;
	}
	while ((*(unsigned int *)buf) == 0)
	{
		buf += 0x10;
	}
	bool x = true;
	for (unsigned int z = 0;z < count;z++)
	{
		char n[1000];
		sprintf(n,"%s.%d.cdata",argv[1],z);
		FILE *f2 = fopen(n,"wb");
		while (x)
		{
			unsigned short Flags = *(unsigned short*)buf;
			buf += 2;
			unsigned short Size = *(unsigned short*)buf;
			buf += 6;
			Swap(Flags);
			Swap(Size);
			fwrite(buf-8,Size,1,f2);
			Size -= 8;
			buf += Size;
			if (Flags & 0x8000)
			{
				unsigned int i = *(unsigned int *)buf;
				if (i == 0)
				{
					buf += 64;
				}
				break;
			}
		}
		fclose(f2);
	}
	delete[] buffer;
}
