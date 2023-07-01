<template>
    <page-main>
        <el-form :model="form" label-width="120px" ref="formRef" :rules="rules">
            <el-form-item label="标题" prop="title">
                <el-input v-model="form.title" />
            </el-form-item>
            <el-form-item label="作者" prop="author">
                <el-input v-model="form.author" />
            </el-form-item>
            <el-form-item label="首图上传" prop="image">
                <el-upload class="avatar-uploader" :before-upload="beforeAvatarUpload" :show-file-list="false">
                    <img v-if="form.image" :src="form.image" class="avatar" />
                    <el-icon v-else class="avatar-uploader-icon">
                        <Plus />
                    </el-icon>
                </el-upload>
            </el-form-item>
            <el-form-item label="分类" prop="categoryId">
                <el-config-provider :zIndex="20000">
                    <categorySelector v-model:parentId="form.categoryId" />
                </el-config-provider>
            </el-form-item>

            <el-form-item label="内容" prop="content">
                <div id="content"></div>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" @click="onSubmit">保存</el-button>
                <el-button @click="reset">重置</el-button>
            </el-form-item>
        </el-form>
    </page-main>
</template>
  
<script lang="ts" setup>
import { FormInstance, FormRules, ElMessage, UploadProps } from 'element-plus';
import { reactive, ref, onMounted } from 'vue'
import E from "wangeditor";
import { useRouter } from 'vue-router'
import fileApi from '@/api/modules/file'
import categorySelector from '@/views/category/components/categorySelector.vue'
import api from '@/api/modules/article'
import { Plus } from '@element-plus/icons-vue'
// do not use same name with ref
const router = useRouter()
const form = reactive({
    id: '',
    title: '',
    author: '',
    content: '',
    picture: '',
    categoryId: '',
    image: ''
})
const rules = reactive<FormRules>({
    title: [
        { required: true, message: '必填!', trigger: 'blur' },
    ],
})

const contentEditor = ref()
const formRef = ref<FormInstance>()
const onSubmit = async () => {
    await formRef.value!.validate(async (valid, fields) => {
        if (valid) {
            console.log('submit!', form)
            if (form.id == '') {
                let res = await api.Create(form)
                ElMessage.success("添加成功!")
            } else {
                let res = await api.Modify(form)
                ElMessage.success("修改成功!")
            }
            reset()
            router.push('/article/index')
        }
    })

}
const reset = () => {
    formRef.value?.resetFields()
}
onMounted(async () => {
    let id = router.currentRoute.value.query.id
    form.id = id;
    console.log('id', form.id)
    contentEditor.value = new E("#content");
    contentEditor.value.config.onchange = (html) => {
        form.content = html;
    };
    // 配置 server 接口地址
    contentEditor.value.config.customUploadImg = function (
        resultFiles,
        insertImgFn
    ) {
        resultFiles.forEach((file) => {
            fileApi.upload(file.name, file).then((result) => {
                insertImgFn(fileApi.getFile(encodeURIComponent(result)));
            });
        });
    };
    contentEditor.value.create();
    if (form.id != undefined) {
        let res = (await api.Get(form.id)) as any
        form.id = res.id;
        form.content = res.content;
        form.title = res.title;
        form.author = res.author;
        if (res.picture) {
            form.picture = res.picture
            form.image = fileApi.getFile(res.picture)
        }
        form.categoryId = res.categoryId
        contentEditor.value.txt.html(form.content);
        console.log(form)
    }
})
const beforeAvatarUpload: UploadProps['beforeUpload'] = (rawFile) => {
    console.log('file', rawFile)
    fileApi.upload(rawFile.name, rawFile).then((res: any) => {
        console.log(res)
        form.picture = res
        form.image = fileApi.getFile(res)
    })
    return false;
}

</script>

<style scoped>
.avatar-uploader .avatar {
    width: 178px;
    height: 178px;
    display: block;
}
</style>

<style>
.avatar-uploader .el-upload {
    border: 1px dashed var(--el-border-color);
    border-radius: 6px;
    cursor: pointer;
    position: relative;
    overflow: hidden;
    transition: var(--el-transition-duration-fast);
}

.avatar-uploader .el-upload:hover {
    border-color: var(--el-color-primary);
}

.el-icon.avatar-uploader-icon {
    font-size: 28px;
    color: #8c939d;
    width: 178px;
    height: 178px;
    text-align: center;
}
</style>