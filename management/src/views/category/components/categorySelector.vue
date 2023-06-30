<template>
    <el-tree-select v-model="treeData.value" :data="treeData.data" check-strictly :render-after-expand="true"
        :default-expand-all="true" />
</template>
  
<script lang="ts" setup>
import { ref, reactive, onMounted, watch } from 'vue'
import api from '@/api/modules/category'

const treeData = reactive({
    data: [{
        value: '0',
        label: '根目录',
        children: []
    }],
    value: ''
})
const props = defineProps({
    parentId: String
})
onMounted(async () => {
    treeData.value = props.parentId
    console.log('ttttt')
    const data = await (api.GetAllCategories()) as any
    ToTreeList(data, treeData.data[0].children, '0')

})
const emit = defineEmits(['update:parentId'])
watch(() => treeData.value, (val) => {
    emit('update:parentId', val)
})
watch(() => props.parentId, (val) => {
    treeData.value = props.parentId
})
const ToTreeList = (list: any, tree: any, parentId: any) => {
    list.forEach((item: any) => {
        // 判断是否为父级菜单
        if (item.parentId === parentId) {
            const child = {
                ...item,
                value: item.id,
                label: item.name,
                children: [],
            };
            // 迭代 list， 找到当前菜单相符合的所有子菜单
            ToTreeList(list, child.children, item.id);
            // 删掉不存在 children 值的属性
            if (child.children.length <= 0) {
                delete child.children;
            }
            // 加入到树中
            tree.push(child);
        }
    });
}
</script>